using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendSis7.Migrations
{
    /// <inheritdoc />
    public partial class egg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SueldoFinal",
                table: "Sueldos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SueldoFinal",
                table: "Sueldos");
        }
    }
}
