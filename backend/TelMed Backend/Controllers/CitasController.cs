using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelMedAPI.Data;
using TelMedAPI.Models;
using FluentValidation;
using TelMedAPI.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TelMedAPI.Helpers;

namespace TelMedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly TelMedAPIContext _context;
        private readonly IValidator<CreateCitaDTO> _validator;

        public CitasController(TelMedAPIContext context, IValidator<CreateCitaDTO> validator)
        {
            _context = context;
            _validator = validator;
        }

        // =========================================================
        // DASHBOARD
        [Authorize(Roles = Roles.Admin + "," + Roles.Secretaria)]
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var hoyInicio = DateTime.UtcNow.Date;
            var hoyFin = hoyInicio.AddDays(1);

            var resumen = new
            {
                Pendientes = await _context.Citas.CountAsync(c => c.Estado == CitaEstados.Pendiente),
                Confirmadas = await _context.Citas.CountAsync(c => c.Estado == CitaEstados.Confirmada),
                Canceladas = await _context.Citas.CountAsync(c => c.Estado == CitaEstados.Cancelada),
                Hoy = await _context.Citas.CountAsync(c => c.FechaInicio >= hoyInicio && c.FechaInicio < hoyFin)
            };

            return Ok(resumen);
        }

        // =========================================================
        // LISTAR CITAS
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCitas()
        {
            var citas = await _context.Citas
                .Include(c => c.Paciente)
                .OrderBy(c => c.FechaInicio)
                .Select(c => new CitaCalendarDto
                {
                    IdCita = c.IdCita,
                    Titulo = c.Motivo,
                    Start = c.FechaInicio,
                    End = c.FechaFin,
                    Estado = c.Estado,
                    PacienteNombreCompleto = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                    TipoConsulta = c.TipoConsulta,
                    PacienteId = c.PacienteId,
                    LinkReunion = c.LinkReunion
                })
                .ToListAsync();

            return Ok(citas);
        }

        // =========================================================
        // CITA POR ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCitaById(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Where(c => c.IdCita == id)
                .Select(c => new CitaCalendarDto
                {
                    IdCita = c.IdCita,
                    Titulo = c.Motivo,
                    Start = c.FechaInicio,
                    End = c.FechaFin,
                    Estado = c.Estado,
                    PacienteNombreCompleto = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                    TipoConsulta = c.TipoConsulta,
                    PacienteId = c.PacienteId,
                    LinkReunion = c.LinkReunion
                })
                .FirstOrDefaultAsync();

            if (cita == null)
                return NotFound();

            return Ok(cita);
        }

        // =========================================================
        // CALENDARIO
        [Authorize(Roles = Roles.Admin + "," + Roles.Secretaria)]
        [HttpGet("calendario")]
        public async Task<ActionResult<IEnumerable<CitaCalendarDto>>> GetCitasCalendario()
        {
            var citas = await _context.Citas
                .Include(c => c.Paciente)
                .OrderBy(c => c.FechaInicio)
                .Select(c => new CitaCalendarDto
                {
                    IdCita = c.IdCita,
                    Titulo = c.Motivo,
                    Start = c.FechaInicio,
                    End = c.FechaFin,
                    Estado = c.Estado,
                    PacienteNombreCompleto = $"{c.Paciente.Nombre} {c.Paciente.Apellido}",
                    TipoConsulta = c.TipoConsulta,
                    PacienteId = c.PacienteId,
                    LinkReunion = c.LinkReunion
                })
                .ToListAsync();

            return Ok(citas);
        }

        // =========================================================
        // INICIAR CONSULTA
        [Authorize]
        [HttpPut("{id}/iniciar")]
        public async Task<IActionResult> IniciarConsulta(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
                return NotFound();

            cita.Estado = CitaEstados.EnConsulta;

            await _context.SaveChangesAsync();

            return Ok(cita);
        }

        // =========================================================
        // FINALIZAR CONSULTA
        [Authorize]
        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> FinalizarConsulta(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
                return NotFound();

            cita.Estado = CitaEstados.Finalizada;

            await _context.SaveChangesAsync();

            return Ok(cita);
        }
    }
}