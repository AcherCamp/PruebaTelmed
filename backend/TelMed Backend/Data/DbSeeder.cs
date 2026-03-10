using TelMedAPI.Models;
using TelMedAPI.Helpers;
using BCrypt.Net;

namespace TelMedAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(TelMedAPIContext context)
        {
            // Si ya existen pacientes o citas no hace nada
            if (context.Usuarios.Any(u => u.Rol == Roles.Paciente) || context.Citas.Any())
                return;

            var pacientes = new List<Usuario>
            {
                new Usuario
                {
                    Nombre = "Carlos",
                    Apellido = "Lopez",
                    Email = "carlos@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Rol = Roles.Paciente,
                    FechaNacimiento = new DateOnly(1990,1,1),
                    Genero = "Masculino",
                    Direccion = "San Miguel",
                    Telefono = "+50370000001"
                },
                new Usuario
                {
                    Nombre = "Maria",
                    Apellido = "Perez",
                    Email = "maria@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Rol = Roles.Paciente,
                    FechaNacimiento = new DateOnly(1995,5,10),
                    Genero = "Femenino",
                    Direccion = "San Miguel",
                    Telefono = "+50370000002"
                },
                new Usuario
                {
                    Nombre = "Jose",
                    Apellido = "Ramirez",
                    Email = "jose@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Rol = Roles.Paciente,
                    FechaNacimiento = new DateOnly(1988,8,8),
                    Genero = "Masculino",
                    Direccion = "Usulutan",
                    Telefono = "+50370000003"
                },
                new Usuario
                {
                    Nombre = "Ana",
                    Apellido = "Torres",
                    Email = "ana@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Rol = Roles.Paciente,
                    FechaNacimiento = new DateOnly(2000,3,15),
                    Genero = "Femenino",
                    Direccion = "La Union",
                    Telefono = "+50370000004"
                },
                new Usuario
                {
                    Nombre = "Luis",
                    Apellido = "Martinez",
                    Email = "luis@test.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Rol = Roles.Paciente,
                    FechaNacimiento = new DateOnly(1992,11,20),
                    Genero = "Masculino",
                    Direccion = "San Salvador",
                    Telefono = "+50370000005"
                }
            };

            context.Usuarios.AddRange(pacientes);
            context.SaveChanges();

            var doctor = context.Usuarios.FirstOrDefault(u => u.Rol == Roles.Admin);

            if (doctor == null)
                return;

            var baseDate = DateTimeOffset.UtcNow;

            var citas = new List<Cita>
            {
                new Cita {PacienteId = pacientes[0].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddHours(8), FechaFin = baseDate.AddHours(9), Motivo="Chequeo", TipoConsulta="General", Estado=CitaEstados.Pendiente},
                new Cita {PacienteId = pacientes[1].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddHours(9), FechaFin = baseDate.AddHours(10), Motivo="Dolor cabeza", TipoConsulta="General", Estado=CitaEstados.Pendiente},
                new Cita {PacienteId = pacientes[2].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddHours(10), FechaFin = baseDate.AddHours(11), Motivo="Consulta", TipoConsulta="General", Estado=CitaEstados.Confirmada},
                new Cita {PacienteId = pacientes[3].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddHours(11), FechaFin = baseDate.AddHours(12), Motivo="Chequeo", TipoConsulta="General", Estado=CitaEstados.Cancelada},
                new Cita {PacienteId = pacientes[4].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddHours(12), FechaFin = baseDate.AddHours(13), Motivo="Dolor estomacal", TipoConsulta="General", Estado=CitaEstados.Pendiente},

                new Cita {PacienteId = pacientes[0].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddDays(1).AddHours(8), FechaFin = baseDate.AddDays(1).AddHours(9), Motivo="Control", TipoConsulta="General", Estado=CitaEstados.Pendiente},
                new Cita {PacienteId = pacientes[1].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddDays(1).AddHours(9), FechaFin = baseDate.AddDays(1).AddHours(10), Motivo="Chequeo", TipoConsulta="General", Estado=CitaEstados.Pendiente},
                new Cita {PacienteId = pacientes[2].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddDays(1).AddHours(10), FechaFin = baseDate.AddDays(1).AddHours(11), Motivo="Consulta", TipoConsulta="General", Estado=CitaEstados.Confirmada},
                new Cita {PacienteId = pacientes[3].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddDays(2).AddHours(11), FechaFin = baseDate.AddDays(2).AddHours(12), Motivo="Dolor muscular", TipoConsulta="General", Estado=CitaEstados.Pendiente},
                new Cita {PacienteId = pacientes[4].Id, DoctorId = doctor.Id, FechaInicio = baseDate.AddDays(2).AddHours(12), FechaFin = baseDate.AddDays(2).AddHours(13), Motivo="Chequeo", TipoConsulta="General", Estado=CitaEstados.Pendiente}
            };

            context.Citas.AddRange(citas);
            context.SaveChanges();
            Console.WriteLine("✅ Seeder completado.");
        }
    }
}