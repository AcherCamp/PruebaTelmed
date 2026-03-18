using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelMedAPI.Data;
using TelMedAPI.DTOs;
using TelMedAPI.Models;
using TelMedAPI.Services;
using QuestPDF.Fluent;

namespace TelMedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasController : ControllerBase
    {
        private readonly TelMedAPIContext _context;

        public ConsultasController(TelMedAPIContext context)
        {
            _context = context;
        }

        [HttpGet("paciente/{pacienteId}")]
        public async Task<IActionResult> GetHistorial(int pacienteId)
        {
            var consultas = await _context.Consultas
                .Include(c => c.Cita)
                .ThenInclude(c => c.Doctor)
                .Where(c => c.Cita != null && c.Cita.PacienteId == pacienteId)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            return Ok(consultas);
        }

        [HttpPost]
        public async Task<IActionResult> CrearConsulta(CreateConsultaDTO dto)
        {
            // 1. Guardar consulta
            var consulta = new Consulta
            {
                CitaId = dto.CitaId,
                Fecha = DateTime.Now,
                Sintomas = dto.Sintomas,
                Evolucion = dto.Evolucion,
                Diagnostico = dto.Diagnostico,
                Tratamiento = dto.Tratamiento,
                Observaciones = dto.Observaciones,
                MedicamentosJson = dto.MedicamentosJson
            };

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            // 2. Traer la cita con paciente
            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(c => c.IdCita == dto.CitaId);

            if (cita == null)
                return NotFound("Cita no encontrada");

            // 3. Generar PDF
            var document = new CitaReport(cita, consulta);
            var pdf = document.GeneratePdf();

            // 4. Devolver PDF
            return File(pdf, "application/pdf", $"Consulta_{cita.IdCita}.pdf");
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            var consulta = await _context.Consultas
                .Include(c => c.Cita)
                .ThenInclude(c => c.Paciente)
                .Include(c => c.Cita.Doctor)
                .FirstOrDefaultAsync(c => c.IdConsulta == id);

            if (consulta == null)
                return NotFound();

            var document = new CitaReport(consulta.Cita, consulta);
            var pdf = document.GeneratePdf();

            return File(pdf, "application/pdf", $"Consulta_{id}.pdf");
        }
    }
}