using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class AddCarrinhoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIMc1oeXBBNBbVanRnl7tBs/O/YaRzveoqN3fehtWMWvdOT+1fqK5MnlQa7MxvcKqg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAUfyuAcC1hNAzsm/2+3nYEpdWK3ZAX8i0E2sZ7XyxL7cfgUSOrg0la+uCaJzVoz2w==");
        }
    }
}
