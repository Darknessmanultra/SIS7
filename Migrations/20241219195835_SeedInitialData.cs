using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSis7.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    nombre = table.Column<string>(type: "TEXT", nullable: false),
                    passwordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.email);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    IdTrabajador = table.Column<Guid>(type: "TEXT", nullable: false),
                    nombre = table.Column<string>(type: "TEXT", nullable: false),
                    CargasFamiliares = table.Column<int>(type: "INTEGER", nullable: false),
                    isapreNombre = table.Column<string>(type: "TEXT", nullable: false),
                    AFPNombre = table.Column<string>(type: "TEXT", nullable: false),
                    tipoContrato = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.IdTrabajador);
                });

            migrationBuilder.CreateTable(
                name: "Sueldos",
                columns: table => new
                {
                    IdTrabajador = table.Column<Guid>(type: "TEXT", nullable: false),
                    Mes = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    SueldoBase = table.Column<int>(type: "INTEGER", nullable: false),
                    HorasExtra = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sueldos", x => x.IdTrabajador);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Sueldos");
        }
    }
}
