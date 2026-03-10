using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TelMedAPI.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("Género")]
        public string Genero { get; set; }

        [Column("fechanacimiento")]
        public DateOnly FechaNacimiento { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("debe_cambiar_password")]
        public bool DebeCambiarPassword { get; set; } = false;

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("rol")]
        public string Rol { get; set; }

        [Column ("admin_id")]
        public string? AdminId {get; set;}

        //Relaciones
        // Citas como paciente
        public ICollection<Cita> CitasComoPaciente { get; set; }
        // Citas como doctor
        public ICollection<Cita> CitasComoDoctor { get; set; }
    }
}