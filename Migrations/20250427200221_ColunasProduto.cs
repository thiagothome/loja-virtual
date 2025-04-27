using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class ColunasProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Produtos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Estoque",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFgFmxYg4eG0mhUgTk0vHT1jyR9TQ2b9IwMdS+ypumqUE7ecQRNtQsHw/o/EbACB/g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Estoque",
                table: "Produtos");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPnvjcOjnCWa3i5EbnV+qLnCU3J8vxlItHeVXEwwBtqOX6oxuhPWpOyKypeEsUJVwQ==");
        }
    }
}
