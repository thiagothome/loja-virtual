using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposPagamentoAoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataPagamento",
                table: "Pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetodoPagamento",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "PasswordHash", "SecurityStamp", "TokenExpiration" },
                values: new object[] { "b3f98f79-5408-47dd-b937-6d89cde31f23", new DateTime(2025, 4, 21, 14, 37, 16, 585, DateTimeKind.Utc).AddTicks(9220), "AQAAAAIAAYagAAAAEEV8aKRURZ7AGrfQ5jr8DcppKTnR/cBlTYq5RKfGCUaMAZotfr5WCzhOUb0+BDzOYg==", "3cef6714-2ebd-4a2a-83cb-e87988b75157", new DateTime(2026, 4, 21, 14, 37, 16, 585, DateTimeKind.Utc).AddTicks(9223) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPagamento",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "MetodoPagamento",
                table: "Pedidos");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "PasswordHash", "SecurityStamp", "TokenExpiration" },
                values: new object[] { "6841c723-b6bf-41db-9928-75e4b8eb10da", new DateTime(2025, 4, 20, 22, 17, 13, 951, DateTimeKind.Utc).AddTicks(7500), "AQAAAAIAAYagAAAAEL49jYh6EoS0veyyPFrAAtHjKhMRj11TBxwNkDWbTx5HlDKeQB9iPSLHBBgT3ASuTg==", "ccc20859-cc03-4efe-92dd-6818a7c5082a", new DateTime(2026, 4, 20, 22, 17, 13, 951, DateTimeKind.Utc).AddTicks(7510) });
        }
    }
}
