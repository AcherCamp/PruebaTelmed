
namespace TelMedAPI.DTOs
{
    public class CreateCitaDTO
    {
        public DateTimeOffset FechaInicio { get; set; }
        public DateTimeOffset FechaFin { get; set; }
        public string Motivo { get; set; }
        public string TipoConsulta { get; set; }
        public int PacienteId { get; set; }
    }
}