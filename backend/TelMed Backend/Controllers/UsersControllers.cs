using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelMedAPI.Models;
using TelMedAPI.Data;
using TelMedAPI.Helpers;

namespace TelMedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly TelMedAPIContext _context;

        public UsersController(TelMedAPIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Apellido,
                    u.Email,
                    u.Rol,
                    u.Telefono,
                    u.Genero
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _context.Usuarios
                .Where(u => u.Rol == Roles.Doctor)
                .Select(u => new
                {
                    Id = u.Id,
                    NombreCompleto = u.Nombre + " " + u.Apellido,
                    Email = u.Email,
                    Telefono = u.Telefono
                })
                .ToListAsync();

            return Ok(doctors);
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _context.Usuarios
                .Where(u => u.Rol == Roles.Paciente)
                .Select(u => new
                {
                    Id = u.Id,
                    NombreCompleto = u.Nombre + " " + u.Apellido,
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Apellido,
                    u.Email,
                    u.Rol
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}