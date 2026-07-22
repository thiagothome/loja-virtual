using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class PedidoFrete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Frete",
                table: "Pedidos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MelhorEnvioServicoId",
                table: "Pedidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServicoFrete",
                table: "Pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Pedidos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Transportadora",
                table: "Pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELJschcRTAm3rTVrvR7rEZBBdPrCkSRWQtZzeKnEjNt8Al6mxIyWka3yHBofAvH6+Q==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frete",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "MelhorEnvioServicoId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ServicoFrete",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Transportadora",
                table: "Pedidos");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEH0lryN7ptPuCKZR9ssOEIGi/jCudtKZ/TaqUN7jT6IzoKwDuGj9ggKncTZtx3Qxwg==");
        }
    }
}
