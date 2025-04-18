namespace SiteAspas.Models
{
   public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public required string ClienteId { get; set; }
        public decimal Total { get; set; }
        public List<PedidoItem> Itens { get; set; } = new();
        public string Status { get; set; } = "Processando";
    }
}