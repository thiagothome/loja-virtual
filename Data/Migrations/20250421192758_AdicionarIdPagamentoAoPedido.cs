using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarIdPagamentoAoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdPagamento",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "PasswordHash", "SecurityStamp", "TokenExpiration" },
                values: new object[] { "69e0b220-7a46-41fa-a517-fe24fa5b0282", new DateTime(2025, 4, 21, 19, 27, 58, 396, DateTimeKind.Utc).AddTicks(4734), "AQAAAAIAAYagAAAAEK6pXmXUrzLo6Im067WTXxQE7svO3MH9Zmlgf6wpXUbs2cdypfPCrchxh9oInHuINA==", "b0cc4493-c643-462e-8362-9c5084ea767e", new DateTime(2026, 4, 21, 19, 27, 58, 396, DateTimeKind.Utc).AddTicks(4741) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPagamento",
                table: "Pedidos");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "PasswordHash", "SecurityStamp", "TokenExpiration" },
                values: new object[] { "b3f98f79-5408-47dd-b937-6d89cde31f23", new DateTime(2025, 4, 21, 14, 37, 16, 585, DateTimeKind.Utc).AddTicks(9220), "AQAAAAIAAYagAAAAEEV8aKRURZ7AGrfQ5jr8DcppKTnR/cBlTYq5RKfGCUaMAZotfr5WCzhOUb0+BDzOYg==", "3cef6714-2ebd-4a2a-83cb-e87988b75157", new DateTime(2026, 4, 21, 14, 37, 16, 585, DateTimeKind.Utc).AddTicks(9223) });
        }
    }
}
