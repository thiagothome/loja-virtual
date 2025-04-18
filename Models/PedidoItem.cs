namespace SiteAspas.Models
{
    public class PedidoItem
{
    public int Id { get; set; }
    public int ProdutoId { get; set; } 
    public Produto Produto { get; set; } 
    public string Nome { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; }
}
}