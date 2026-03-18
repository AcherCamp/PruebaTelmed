using TelMedAPI.Models;
using TelMedAPI.Helpers;
using BCrypt.Net;

namespace TelMedAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(TelMedAPIContext context)
        {
            if (context.Citas.Any())
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

            //Creación de citas
            var doctor = context.Usuarios.FirstOrDefault(u => u.Rol == Roles.Admin);

            if (doctor == null)
            {
                Console.WriteLine("⚠️ No hay doctor admin.");
                return;
            }

            // buscar próximo lunes
            var today = DateTimeOffset.UtcNow.Date;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            var monday = today.AddDays(daysUntilMonday);

            var estados = new[]
            {
                CitaEstados.Pendiente,
                CitaEstados.Confirmada,
                CitaEstados.Cancelada
            };

            var random = new Random();

            var citas = new List<Cita>();

            // lunes a viernes
            for (int d = 0; d < 5; d++)
            {
                var day = monday.AddDays(d);

                // horario 9 a 16 (9-10, 10-11...)
                for (int h = 9; h < 17; h++)
                {
                    var paciente = pacientes[(d + h) % pacientes.Count];

                    citas.Add(new Cita
                    {
                        PacienteId = paciente.Id,
                        DoctorId = doctor.Id,
                        FechaInicio = day.AddHours(h).ToUniversalTime(),
                        FechaFin = day.AddHours(h + 1).ToUniversalTime(),
                        Motivo = "Consulta médica",
                        TipoConsulta = "General",
                        Estado = estados[random.Next(estados.Length)]
                    });
                }
            }

            context.Citas.AddRange(citas);
            context.SaveChanges();

            Console.WriteLine($"✅ Seeder completado con {citas.Count} citas generadas.");
        }
    }
}