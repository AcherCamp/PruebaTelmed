using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelMedAPI.Data;
using TelMedAPI.Models;
using FluentValidation;
using QuestPDF.Fluent;
using TelMedAPI.Services;
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
        // Resumen Dashboard para Admin y Secretaria
        [Authorize(Roles = Roles.Admin + "," + Roles.Secretaria)]
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var hoyInicio = DateTime.UtcNow.Date;
            var hoyFin = hoyInicio.AddDays(1);

            var pendientes = await _context.Citas
                .CountAsync(c => c.Estado == CitaEstados.Pendiente);

            var confirmadas = await _context.Citas
                .CountAsync(c => c.Estado == CitaEstados.Confirmada);

            var canceladas = await _context.Citas
                .CountAsync(c => c.Estado == CitaEstados.Cancelada);

            var citasHoy = await _context.Citas
                .CountAsync(c => c.FechaInicio >= hoyInicio &&
                                c.FechaInicio < hoyFin);

            var resumen = new
            {
                Pendientes = pendientes,
                Confirmadas = confirmadas,
                Canceladas = canceladas,
                Hoy = citasHoy
            };

            return Ok(resumen);
        }

        // =========================================================
        // OBTENER CITAS (filtrado por rol + rango de fecha)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCitas(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.Citas
                .Include(c => c.Paciente)
                .AsQueryable();

            if (rol == Roles.Paciente)
            {
                var paciente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (paciente == null)
                    return Unauthorized();

                query = query.Where(c => c.PacienteId == paciente.Id);
            }

            if (fechaInicio.HasValue)
                query = query.Where(c => c.FechaInicio >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(c => c.FechaInicio <= fechaFin.Value);

            var citas = await query
                .OrderBy(c => c.FechaInicio)
                .ToListAsync();

            return Ok(citas);
        }

        //=========================================================================
        // Historial de citas, Admin y Secretaria ven todo, Paciente solo las suyas
        [Authorize]
        [HttpGet("historial")]
        public async Task<IActionResult> GetHistorial()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.Citas
                .Include(c => c.Paciente)
                .AsQueryable();

            if (rol == Roles.Paciente)
            {
                var paciente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (paciente == null)
                    return Unauthorized();

                query = query.Where(c => c.PacienteId == paciente.Id);
            }

            var historial = await query
                .Where(c =>
                    c.Estado == CitaEstados.Cancelada ||
                    c.FechaInicio < DateTime.UtcNow
                )
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();

            return Ok(historial);
        }

        // =========================================================
        // DISPONIBILIDAD
        [HttpGet("disponibilidad")]
        public async Task<IActionResult> ObtenerDisponibilidad([FromQuery] DateTime fecha)
        {
            var fechaUtc = DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
            var diaSemana = fechaUtc.DayOfWeek;

            if (diaSemana == DayOfWeek.Sunday)
                return Ok(new List<string>());

            TimeSpan horaInicio = new TimeSpan(8, 0, 0);
            TimeSpan horaFin = diaSemana == DayOfWeek.Saturday
                ? new TimeSpan(12, 0, 0)
                : new TimeSpan(17, 0, 0);

            var inicioDia = fechaUtc.Date.Add(horaInicio);
            var finDia = fechaUtc.Date.Add(horaFin);

            var citasDelDia = await _context.Citas
                .Where(c => c.FechaInicio >= inicioDia && c.FechaInicio < finDia)
                .ToListAsync();

            var bloquesDisponibles = new List<string>();

            for (var hora = inicioDia; hora < finDia; hora = hora.AddMinutes(30))
            {
                var bloqueFin = hora.AddMinutes(30);

                bool ocupado = citasDelDia.Any(c =>
                    c.FechaInicio < bloqueFin &&
                    c.FechaFin > hora
                );

                if (!ocupado)
                    bloquesDisponibles.Add(hora.ToString("HH:mm"));
            }

            return Ok(bloquesDisponibles);
        }

        // =========================================================
        // CREAR CITA (Paciente o Admin)
        [Authorize(Roles = Roles.Paciente + "," + Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CrearCita([FromBody] CreateCitaDTO dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var admin = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Rol == Roles.Admin);

            if (admin == null)
                return BadRequest("No existe administrador configurado.");

            var cita = new Cita
            {
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Motivo = dto.Motivo,
                TipoConsulta = dto.TipoConsulta,
                PacienteId = dto.PacienteId,
                DoctorId = admin.Id,
                Estado = CitaEstados.Pendiente
            };

            if (cita.FechaInicio < DateTime.UtcNow)
                return BadRequest("No se pueden registrar citas en fechas pasadas.");

            if (cita.FechaFin <= cita.FechaInicio)
                return BadRequest("La fecha de fin debe ser mayor que la fecha de inicio.");

            if (cita.FechaInicio.DayOfWeek == DayOfWeek.Sunday)
                return BadRequest("No se permiten citas los domingos.");

            var inicioPermitido = new TimeSpan(8, 0, 0);
            var finPermitido = new TimeSpan(17, 0, 0);

            if (cita.FechaInicio.TimeOfDay < inicioPermitido ||
                cita.FechaFin.TimeOfDay > finPermitido)
                return BadRequest("Horario fuera del rango permitido.");

            bool existeSolapamiento = await _context.Citas.AnyAsync(c =>
                c.DoctorId == admin.Id &&
                cita.FechaInicio < c.FechaFin &&
                cita.FechaFin > c.FechaInicio
            );

            if (existeSolapamiento)
                return BadRequest("Ya existe una cita en ese rango.");

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return Ok("Cita creada correctamente.");
        }

        // =========================================================
        // EDITAR CITA (Admin y Secretaria)
        [Authorize(Roles = Roles.Admin + "," + Roles.Secretaria)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCita(int id, [FromBody] UpdateCitaDTO dto)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
                return NotFound("Cita no encontrada.");

            if (cita.Estado != CitaEstados.Pendiente)
                return BadRequest("Solo se pueden editar citas pendientes.");

            cita.FechaInicio = dto.FechaInicio;
            cita.FechaFin = dto.FechaFin;
            cita.Motivo = dto.Motivo;
            cita.TipoConsulta = dto.TipoConsulta;

            await _context.SaveChangesAsync();

            return Ok("Cita actualizada correctamente.");
        }

        // =========================================================
        // CONFIRMAR CITA
        [Authorize(Roles = Roles.Admin + "," + Roles.Secretaria)]
        [HttpPut("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
                return NotFound("Cita no encontrada.");

            if (cita.Estado != CitaEstados.Pendiente)
                return BadRequest("Solo se pueden confirmar citas pendientes.");

            cita.Estado = CitaEstados.Confirmada;

            await _context.SaveChangesAsync();

            return Ok("Cita confirmada correctamente.");
        }

        // =========================================================
        // CANCELAR CITA (Paciente, Admin, Secretaria)
        [Authorize]
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarCita(int id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
                return NotFound("Cita no encontrada.");

            if (rol == Roles.Paciente)
            {
                var paciente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (paciente == null || cita.PacienteId != paciente.Id)
                    return Unauthorized();

                if (cita.Estado != CitaEstados.Pendiente)
                    return BadRequest("Solo puedes cancelar citas pendientes.");
            }

            cita.Estado = CitaEstados.Cancelada;

            await _context.SaveChangesAsync();

            return Ok("Cita cancelada correctamente.");
        }

        // =========================================================
        // PDF
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
                return NotFound();

            var document = new CitaReport(cita);
            var pdfData = document.GeneratePdf();

            return File(pdfData, "application/pdf", $"Cita_{id}.pdf");
        }
    }
}