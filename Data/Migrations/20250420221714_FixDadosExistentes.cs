using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDadosExistentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "PasswordHash", "SecurityStamp", "TokenExpiration" },
                values: new object[] { "6841c723-b6bf-41db-9928-75e4b8eb10da", new DateTime(2025, 4, 20, 22, 17, 13, 951, DateTimeKind.Utc).AddTicks(7500), "AQAAAAIAAYagAAAAEL49jYh6EoS0veyyPFrAAtHjKhMRj11TBxwNkDWbTx5HlDKeQB9iPSLHBBgT3ASuTg==", "ccc20859-cc03-4efe-92dd-6818a7c5082a", new DateTime(2026, 4, 20, 22, 17, 13, 951, DateTimeKind.Utc).AddTicks(7510) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "PasswordHash", "SecurityStamp", "TokenExpiration" },
                values: new object[] { "0d2ef7e8-5515-4cee-9511-8fa9bea26e26", new DateTime(2025, 4, 20, 22, 16, 43, 500, DateTimeKind.Utc).AddTicks(6439), "AQAAAAIAAYagAAAAEENiSPIbaaFIzf+XZ2Hpq21jckNamelMQTHnClJorG+JcFUBn4GeEWHW8a1cMKhlfw==", "c98a5ab9-e0b0-4b1a-8e91-ad04e84aae0e", new DateTime(2026, 4, 20, 22, 16, 43, 500, DateTimeKind.Utc).AddTicks(6445) });
        }
    }
}
