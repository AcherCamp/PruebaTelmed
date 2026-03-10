using System.ComponentModel.DataAnnotations;

namespace TelMedAPI.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Genero { get; set; }

        [Required]
        public DateOnly FechaNacimiento { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Telefono { get; set; }
    }
}