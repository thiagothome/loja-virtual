using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionadoPropsPix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCodeBase64",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPnvjcOjnCWa3i5EbnV+qLnCU3J8vxlItHeVXEwwBtqOX6oxuhPWpOyKypeEsUJVwQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "QrCodeBase64",
                table: "Pedidos");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHqbZIzN4gOcwLAXJauoXwDRbXvXk/PDxM5OUN0EUANvoAxxFWFubtx9U2WE6rSeiw==");
        }
    }
}
