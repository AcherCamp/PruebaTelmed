using TelMedAPI.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Genero { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public string Direccion { get; set; }

    public string Email { get; set; }
    public string PasswordHash { get; set; }

    public string Telefono { get; set; }
    public string Rol { get; set; }
    public bool DebeCambiarPassword { get; set; } = false;

    // Relaciones
    public ICollection<Cita> CitasComoPaciente { get; set; }
    public ICollection<Cita> CitasComoDoctor { get; set; }
}