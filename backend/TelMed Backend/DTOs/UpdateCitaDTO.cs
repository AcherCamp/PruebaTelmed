namespace TelMedAPI.DTOs
{
    public class UpdateCitaDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; }
        public string TipoConsulta { get; set; }
    }
}