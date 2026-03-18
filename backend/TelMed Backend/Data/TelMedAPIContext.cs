using Microsoft.EntityFrameworkCore;
using TelMedAPI.Models;

namespace TelMedAPI.Data
{
    public class TelMedAPIContext : DbContext
    {
        public TelMedAPIContext(DbContextOptions<TelMedAPIContext> options)
            : base(options)
        {           
        }

        public DbSet<Cita> Citas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Consulta> Consultas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

              // Relación Paciente → Cita
                modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(u => u.CitasComoPaciente)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

                // Relación Doctor → Cita
                modelBuilder.Entity<Cita>()
                .HasOne(c => c.Doctor)
                .WithMany(u => u.CitasComoDoctor)
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
         }
    }
}