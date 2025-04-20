using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaSenhaAdmin3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { 1, null, new DateTime(2025, 4, 20, 14, 54, 30, 696, DateTimeKind.Utc).AddTicks(9595), null, "admin@admin.com", true, "Administrador do Sistema", "AQAAAAIAAYagAAAAEAuQ38nTvCEZibUpXAVPb1jS6quZ/Lk4FbJ03IavBXl8OlnzwVTihyR3o8gPkYBeFQ==", null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
