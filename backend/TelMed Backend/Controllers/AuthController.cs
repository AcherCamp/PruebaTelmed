using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TelMedAPI.Data;
using TelMedAPI.Models;
using TelMedAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using TelMedAPI.Helpers;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TelMedAPIContext _context;
    private readonly string _key;

    public AuthController(TelMedAPIContext context, IConfiguration config)
    {
        _context = context;
        _key = config["Jwt:Key"]
            ?? throw new ArgumentNullException("Jwt:Key no configurado.");
    }

    // =========================================================
    // ADMIN CREA DOCTOR
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("create-doctor")]
    public async Task<IActionResult> CreateDoctor([FromBody] RegisterDTO model)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            return BadRequest("El correo ya está registrado.");

        var doctor = new Usuario
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Genero = model.Genero,
            FechaNacimiento = model.FechaNacimiento,
            Direccion = model.Direccion,
            Email = model.Email,
            Telefono = model.Telefono,
            Rol = Roles.Doctor,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            DebeCambiarPassword = true // 🔥 importante
        };

        _context.Usuarios.Add(doctor);
        await _context.SaveChangesAsync();

        return Ok("Doctor creado correctamente.");
    }

    // =========================================================
    // REGISTRO PACIENTE
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            return BadRequest("El correo ya está registrado.");

        var paciente = new Usuario
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Genero = model.Genero,
            FechaNacimiento = model.FechaNacimiento,
            Direccion = model.Direccion,
            Email = model.Email,
            Telefono = model.Telefono,
            Rol = Roles.Paciente,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            DebeCambiarPassword = false
        };

        _context.Usuarios.Add(paciente);
        await _context.SaveChangesAsync();

        return Ok("Paciente registrado correctamente.");
    }

    // =========================================================
    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == login.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            return Unauthorized("Credenciales inválidas");

        var token = GenerarToken(user);

        return Ok(new
        {
            token,
            rol = user.Rol,
            requiereCambioPassword = user.DebeCambiarPassword
        });
    }

    // =========================================================
    // PERFIL
    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> Perfil()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var user = await _context.Usuarios
            .Where(u => u.Email == email)
            .Select(u => new
            {
                u.Nombre,
                u.Apellido,
                u.Email,
                u.Rol
            })
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();

        return Ok(user);
    }

    // =========================================================
    // CAMBIAR PASSWORD
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(model.PasswordActual, user.PasswordHash))
            return BadRequest("Password actual incorrecto.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordNueva);
        user.DebeCambiarPassword = false;

        await _context.SaveChangesAsync();

        return Ok("Password actualizado.");
    }

    // =========================================================
    // GENERAR TOKEN
    private string GenerarToken(Usuario user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Rol)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = user.DebeCambiarPassword
                ? DateTime.UtcNow.AddMinutes(15)
                : DateTime.UtcNow.AddHours(2),

            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }
}