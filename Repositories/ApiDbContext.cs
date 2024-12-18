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
}