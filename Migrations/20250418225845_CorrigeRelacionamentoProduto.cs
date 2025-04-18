using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteAspas.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeRelacionamentoProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItems_Pedidos_PedidoId",
                table: "PedidoItems");

            migrationBuilder.AlterColumn<int>(
                name: "PedidoId",
                table: "PedidoItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CarrinhoItems",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagemUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItems_ProdutoId",
                table: "PedidoItems",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItems_Pedidos_PedidoId",
                table: "PedidoItems",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItems_Produtos_ProdutoId",
                table: "PedidoItems",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItems_Pedidos_PedidoId",
                table: "PedidoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItems_Produtos_ProdutoId",
                table: "PedidoItems");

            migrationBuilder.DropTable(
                name: "CarrinhoItems");

            migrationBuilder.DropIndex(
                name: "IX_PedidoItems_ProdutoId",
                table: "PedidoItems");

            migrationBuilder.AlterColumn<int>(
                name: "PedidoId",
                table: "PedidoItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItems_Pedidos_PedidoId",
                table: "PedidoItems",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id");
        }
    }
}
