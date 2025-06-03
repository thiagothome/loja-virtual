namespace SiteAspas.Models
{
    public class CarrinhoItem
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public decimal? Preco { get; set; }
        public int Quantidade { get; set; }
        public int ClienteId { get; set; }
        public DateTime DataAdicao { get; set; } = DateTime.UtcNow;
        public decimal? Altura { get; set; }

        public decimal? Largura { get; set; }

        public decimal? Comprimento { get; set; }

        public decimal? Peso { get; set; }
    }
}