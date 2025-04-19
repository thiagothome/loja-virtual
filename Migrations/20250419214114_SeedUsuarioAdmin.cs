using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuarioAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "CPF", "DataCadastro", "DataNascimento", "Email", "IsAtivo", "NomeCompleto", "Senha", "Telefone", "Tipo" },
                values: new object[] { 1, null, new DateTime(2025, 4, 19, 21, 41, 14, 300, DateTimeKind.Utc).AddTicks(3860), null, "admin@admin.com", true, "Administrador do Sistema", "Admin123", null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Usuarios");
        }
    }
}
