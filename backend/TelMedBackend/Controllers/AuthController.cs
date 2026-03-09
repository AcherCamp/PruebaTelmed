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
using System.Collections.Generic;

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
         ?? throw new ArgumentNullException("Jwt:Key no está configurado en appsettings.json");
    }

    // =========================================================
    // ADMIN CREA SECRETARIA
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("create-secretaria")]
    public async Task<IActionResult> CreateSecretaria([FromBody] RegisterDTO model)
    {
        var existe = await _context.Usuarios
            .AnyAsync(u => u.Email == model.Email);

        if (existe)
            return BadRequest("El correo ya está registrado.");

        var nuevaSecretaria = new Usuario
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Genero = model.Genero,
            FechaNacimiento = model.FechaNacimiento,
            Direccion = model.Direccion,
            Email = model.Email,
            Telefono = model.Telefono,
            Rol = Roles.Secretaria,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            DebeCambiarPassword = false
        };

        await _context.Usuarios.AddAsync(nuevaSecretaria);
        await _context.SaveChangesAsync();

        return Ok("Secretaria creada correctamente.");
    }

    // =========================================================
    // CAMBIO DE PASSWORD
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(model.PasswordActual, user.Password))
            return BadRequest("Password actual incorrecto.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(model.PasswordNueva);
        user.DebeCambiarPassword = false;

        await _context.SaveChangesAsync();

        return Ok("Password actualizado correctamente.");
    }

    // =========================================================
    // PERFIL DEL USUARIO AUTENTICADO
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

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // =========================================================
    // REGISTRO PÚBLICO (SOLO PACIENTE)
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var existe = await _context.Usuarios
            .AnyAsync(u => u.Email == model.Email);

        if (existe)
            return BadRequest("El correo ya está registrado.");

        var nuevoPaciente = new Usuario
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Genero = model.Genero,
            FechaNacimiento = model.FechaNacimiento,
            Direccion = model.Direccion,
            Email = model.Email,
            Telefono = model.Telefono,
            Rol = Roles.Paciente, // Forzamos rol paciente
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            DebeCambiarPassword = false
        };

        await _context.Usuarios.AddAsync(nuevoPaciente);
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

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
                return Unauthorized("Credenciales inválidas");

            var token = GenerarToken(user);

            if (user.DebeCambiarPassword)
            {
                return Ok(new
                {
                    mensaje = "Debe cambiar su contraseña antes de continuar.",
                    token = token,
                    requiereCambioPassword = true
                });
            }

            return Ok(new
            {
                token = token,
                rol = user.Rol,
                requiereCambioPassword = false
            });
        }

        // =========================================================
        // MÉTODO PRIVADO PARA GENERAR TOKEN
        private string GenerarToken(Usuario user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            if (user.DebeCambiarPassword)
            {
                claims.Add(new Claim("MustChangePassword", "true"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Si el usuario debe cambiar su password, el token expira en 15 minutos, sino en 2 horas
                Expires = user.DebeCambiarPassword
                ? DateTime.UtcNow.AddMinutes(15)
                :DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
}

