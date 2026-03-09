using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelMedAPI.Models;
using TelMedAPI.Data;
using System.Security.Claims;

namespace TelMedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]  // ← Solo admins pueden ver/editar usuarios
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TelMedAPIContext _context;  // si necesitas joins o más datos

        public UsersController(
            UserManager<Usuario> userManager,
            TelMedAPIContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/users (lista todos los usuarios con roles)
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Apellido,
                    u.Email,
                    u.Rol,                // tu campo custom
                    u.Telefono,
                    u.Genero,
                    // Si quieres roles de Identity (por si usas múltiples)
                    // Roles = await _userManager.GetRolesAsync(u)
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/doctors (solo doctores, útil para asignar citas)
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _userManager.Users
                .Where(u => u.Rol == "Doctor")  // o Roles.Doctor si es constante
                .Select(u => new
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Email = u.Email,
                    Telefono = u.Telefono
                })
                .ToListAsync();

            return Ok(doctors);
        }

        // GET: api/users/patients (similar para pacientes)
        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _userManager.Users
                .Where(u => u.Rol == "Paciente")
                .Select(u => new
                {
                    Id = u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(patients);
        }

        // Opcional: GET api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.Nombre,
                user.Apellido,
                user.Email,
                user.Rol,
            });
        }
    }
}