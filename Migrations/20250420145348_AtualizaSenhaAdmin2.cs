using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaSenhaAdmin2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Senha",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Senha",
                table: "Usuarios",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "CPF", "DataCadastro", "DataNascimento", "Email", "IsAtivo", "NomeCompleto", "Senha", "Telefone", "Tipo" },
                values: new object[] { 1, null, new DateTime(2025, 4, 20, 14, 49, 13, 757, DateTimeKind.Utc).AddTicks(7109), null, "admin@admin.com", true, "Administrador do Sistema", "Admin123", null, 1 });
        }
    }
}
