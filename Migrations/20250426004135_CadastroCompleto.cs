using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class CadastroCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CadastroCompleto",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CadastroCompleto", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAEHqbZIzN4gOcwLAXJauoXwDRbXvXk/PDxM5OUN0EUANvoAxxFWFubtx9U2WE6rSeiw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CadastroCompleto",
                table: "Usuarios");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE79zlwOqw4NCdC2LZT+06FCYSO/r5wBVPgWj3KB6du/VXPzp7EDGVVl5Wtvcq+MjQ==");
        }
    }
}
