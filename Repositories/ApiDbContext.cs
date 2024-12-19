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
            if (!context.Admin.Any())
            {
                context.Admin.Add(
                    new Administrador
                    {
                    nombre="README.md",
                    email="algo@algo.com",
                    passwordHash=BCrypt.Net.BCrypt.HashPassword("admin123")
                    }
                );
            }
            if (!context.Empleados.Any()&&!context.Sueldos.Any())
            {
                Guid guiso= Guid.NewGuid();
                context.Empleados.Add(
                    new Empleado
                    {
                    IdTrabajador=guiso,
                    nombre="Armando Mesas",
                    CargasFamiliares=1,
                    isapreNombre="BANMEDICA",
                    AFPNombre="PROVIDA",
                    tipoContrato="PLAZO"
                    }
                );
                context.Sueldos.Add(
                    new Sueldo
                    {
                    IdTrabajador=guiso,
                    SueldoBase=8000,
                    HorasExtra=8,
                    Mes=DateOnly.Parse("01-01-2020")
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}