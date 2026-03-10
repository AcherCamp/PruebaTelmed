using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TelMedAPI.Helpers;

namespace TelMedAPI.Models
{
    [Table("cita")]
    public class Cita
    {
        [Key]
        [Column("id_cita")]
        public int IdCita { get; set; }

        [Column("fecha_inicio")]
        public DateTimeOffset FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTimeOffset FechaFin { get; set; }

        [Column("motivo")]
        public string Motivo { get; set; } = string.Empty;

        [Column("tipo_consulta")]
        public string TipoConsulta { get; set; } = string.Empty;

        [Column("estado")]
        public string Estado { get; set; } = CitaEstados.Pendiente;

        // PACIENTE
        [Column("paciente_id")]
        public int PacienteId { get; set; }

        [ForeignKey("PacienteId")]
        public Usuario Paciente { get; set; } = null!;

        // DOCTOR
        [Column("doctor_id")]
        public int? DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public Usuario? Doctor { get; set; }
    }
}