using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TelMedAPI.Data;
using TelMedAPI.Models;

namespace TelMedAPI.Validators
{
    public class CitaValidator : AbstractValidator<Cita>
    {
        private readonly TelMedAPIContext _context;

        public CitaValidator(TelMedAPIContext context)
        {
            _context = context;

          RuleFor(c => c.PacienteId)
         .GreaterThan(0).WithMessage("Debe seleccionar un paciente válido.");

            RuleFor(c => c.FechaInicio)
                .LessThan(c => c.FechaFin)
                .WithMessage("La fecha de inicio debe ser menor que la fecha de fin.");

            RuleFor(c => c)
                .MustAsync(NoSolapamiento)
                .WithMessage("Ya existe una cita en ese rango de horario.");
        }

       private async Task<bool> NoSolapamiento(Cita cita, CancellationToken cancellation)
        {
            return !await _context.Citas.AnyAsync(x =>
                x.DoctorId == cita.DoctorId &&
                x.FechaInicio < cita.FechaFin &&
                x.FechaFin > cita.FechaInicio,
                cancellation
            );
        }
    }
}