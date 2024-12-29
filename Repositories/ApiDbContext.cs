using BackendSis7.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendSis7.Repositories
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) {}
        public DbSet<Empleado> Empleados {get;set;}
        public DbSet<Administrador> Admin {get;set;}
        public DbSet<Sueldo> Sueldos {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
    }

    public static class DbSeeder
    {
        public static async Task SeedAsync(ApiDbContext context)
        {
            // Admin seed permanece igual
            if (!context.Admin.Any())
            {
                context.Admin.Add(
                    new Administrador
                    {
                        nombre = "Wilfredo Soler",
                        email = "wilfredo.soler@admin.com",
                        passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
                    }
                );
            }

            // Seed de empleados y sueldos
            if (!context.Empleados.Any() && !context.Sueldos.Any())
            {
                var nombres = new[] 
                {
                    "Carlos Morales", "Ana María Pérez", "Juan Rodriguez", "María González",
                    "Pedro Sánchez", "Laura Torres", "Diego Ramírez", "Sofia Castro",
                    "Luis Hernández", "Carmen Flores", "Roberto Medina", "Patricia Vargas",
                    "Miguel Ángel Ruiz", "Isabel Jiménez", "Fernando Silva", "Andrea López",
                    "Ricardo Mendoza", "Valentina Reyes", "Gabriel Ortiz", "Carolina Muñoz",
                    "José López", "Lucía Fernández", "Alberto García", "Elena Pérez", 
                    "Francisco Martín", "Paula Sánchez", "Javier Rodríguez", "Marta Díaz", 
                    "Rafael Pérez", "Cecilia Morales", "Carlos Rodríguez", "Sofía Hernández", 
                    "Antonio Ruiz", "Victoria Gómez", "Julio Torres", "Paola Ramírez", 
                    "Pedro Díaz", "José González", "Marina González", "Iván Castro", 
                    "Teresa López", "Eva Martínez", "David Sánchez", "Ana Sánchez", 
                    "Miguel Morales", "Roberta Fernández", "Adrián Pérez", "Julia Martínez",
                    "Luis Sánchez", "Nerea Gómez", "Raúl Torres", "María González", 
                    "Héctor Pérez", "Juan González", "Sandra Ruiz", "Ricardo Sánchez", 
                    "Paula López", "Javier Gómez", "Beatriz Ramírez", "Tomás González", 
                    "Eduardo Díaz", "Lucía García", "Pablo Torres", "Esteban Fernández", 
                    "Natalia López", "Cristina Martínez", "Laura García", "Fernando Rodríguez",
                    "Raquel Pérez", "Carlos Gómez", "Susana Sánchez", "Fabiola Martínez", 
                    "Antonio García", "Inés Pérez", "Isabel García", "Daniel Díaz", 
                    "Rosa Sánchez", "Marcos Rodríguez", "Mónica Fernández", "Joaquín Pérez", 
                    "Elena Gómez", "Cristina Rodríguez", "Ángel López", "Elisa Torres", 
                    "José María González", "Daniela Ramírez", "Paola Fernández", "Antonio Díaz", 
                    "Marina Rodríguez", "Luis Pérez", "Esteban Sánchez", "Verónica López", 
                    "Isabel González", "Guillermo Fernández", "Andrés Díaz", "Carmen García", 
                    "Fernando Pérez", "Silvia González", "Ricardo Díaz", "José María Sánchez", 
                    "Verónica Torres", "Emilia Pérez", "Guillermo Rodríguez", "Álvaro Sánchez", 
                    "Fabiola Pérez", "Mónica López", "Ricardo López", "Paula Gómez", 
                    "Antonio Sánchez", "Verónica Pérez", "Cristina López", "Juan Pérez", 
                    "Elena Rodríguez", "Eduardo Sánchez", "José Ramírez", "María Jiménez", 
                    "Antonio Ramírez", "Raúl García", "Carmen Sánchez", "David Martínez", 
                    "Carolina Díaz", "Rosa Pérez", "Javier Sánchez", "Marta González", 
                    "Carlos Díaz", "Pedro Ramírez", "Miguel González", "Isabel Fernández", 
                    "Sergio Sánchez", "María Ramírez", "José Pérez", "Adriana Martínez", 
                    "José Manuel Sánchez", "Lorena González", "Alejandro Sánchez", "Pedro Sánchez", 
                    "Mónica López", "Alejandro Pérez", "Antonio Fernández", "Eduardo González", 
                    "Felipe López", "Mónica Ramírez", "Pedro Martínez", "Sergio López", 
                    "Cristina Sánchez", "Carlos Martínez", "Javier Pérez", "Juan Jiménez", 
                    "Alba González", "Raquel González", "Luis Ramírez", "Alfredo López", 
                    "Ricardo García", "Ana María Sánchez", "Antonio González", "Rafael Ramírez", 
                    "Pedro Sánchez", "Carlos Fernández", "Antonio Rodríguez", "Laura Pérez", 
                    "Luis López", "Esteban Pérez", "José Sánchez", "José Antonio García", 
                    "José López", "Andrés Ramírez", "José González", "María López", 
                    "Ricardo Ramírez", "Patricia Pérez", "Carlos Rodríguez", "Laura González", 
                    "José Sánchez", "Silvia Pérez", "Marina Sánchez", "Carmen Rodríguez", 
                    "Daniel López", "Martín Fernández", "Pedro González", "Marta López", 
                    "María Fernández", "José María Rodríguez", "Beatriz Sánchez", "David Ramírez", 
                    "Antonio Pérez", "Alfredo Rodríguez", "Nicolás Pérez", "Ana González", 
                    "Álvaro Pérez", "David González", "Marcos Pérez", "Joaquín Ramírez", 
                    "Pedro Fernández", "Cristina Pérez", "Marta Rodríguez", "Ricardo Fernández", 
                    "Antonio Ramírez", "Juan Fernández", "Cristina Ramírez", "José García", 
                    "Silvia González", "Luis González", "María Sánchez", "Carlos Ramírez", 
                    "Javier López", "Mónica Sánchez", "David Pérez", "Patricia González", 
                    "Ana Ramírez", "Rosa Rodríguez", "Pedro Ramírez", "Ricardo Martínez"
                };

                var isapres = new[] { "FONASA", "CONSALUD", "BANMEDICA", "CRUZ BLANCA" };
                var afps = new[] { "MODELO", "PROVIDA", "HABITAT", "CAPITAL", "PLANVITAL" };
                var tiposContrato = new[] { "PLAZO", "INDEFINIDO" };

                var random = new Random();
                var startDate = new DateTime(2022, 1, 1);
                var endDate = new DateTime(2025, 1, 1);

                for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                {
                    // Crear 10 empleados por mes
                    for (int i = 0; i < 10; i++)
                    {
                        var empleadoId = Guid.NewGuid();
                        
                        // Crear empleado
                        context.Empleados.Add(
                            new Empleado
                            {
                                IdTrabajador = empleadoId,
                                nombre = nombres[random.Next(nombres.Length)], // Nombre aleatorio de la lista
                                CargasFamiliares = random.Next(0, 4),
                                isapreNombre = isapres[random.Next(isapres.Length)],
                                AFPNombre = afps[random.Next(afps.Length)],
                                tipoContrato = tiposContrato[random.Next(tiposContrato.Length)]
                            }
                        );

                        // Crear sueldo
                        context.Sueldos.Add(
                            new Sueldo
                            {
                                IdTrabajador = empleadoId,
                                SueldoBase = random.Next(500000, 2500000), // Sueldos entre 500.000 y 2.500.000
                                HorasExtra = random.Next(0, 20), // Entre 0 y 20 horas extra
                                Mes = new DateOnly(date.Year, date.Month, 1) // Fecha correspondiente a cada mes
                            }
                        );
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}


