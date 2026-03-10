using FluentValidation;
using TelMedAPI.DTOs;

namespace TelMedAPI.Validators
{
    public class CreateCitaValidator : AbstractValidator<CreateCitaDTO>
    {
        public CreateCitaValidator()
        {
            RuleFor(x => x.PacienteId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un paciente válido.");

            RuleFor(x => x.FechaInicio)
                .LessThan(x => x.FechaFin)
                .WithMessage("La fecha de inicio debe ser menor que la fecha de fin.");
        
            RuleFor(x => x.Motivo)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.TipoConsulta)
                .NotEmpty();
        
        }
    }
}