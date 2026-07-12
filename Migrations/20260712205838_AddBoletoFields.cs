using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class AddBoletoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoletoUrl",
                table: "Pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinhaDigitavel",
                table: "Pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NossoNumero",
                table: "Pedidos",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKElCpobl1W4p0f3n5Cb7pYFxOkq1T8XGiOenn0At6KIB+fQTQPC5VFEU4BMiOYJSw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoletoUrl",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "LinhaDigitavel",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "NossoNumero",
                table: "Pedidos");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFp6PXaRdIgSawXZvBAM0jN0xHOPeH9o/LxVtmElIX5+fRChU1BVyE6jLn7i4VQeBA==");
        }
    }
}
