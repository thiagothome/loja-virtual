using SiteAspas.Models.Enums;

namespace SiteAspas.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public DateTime DataPedido { get; set; } = DateTime.UtcNow;

        public required int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public int EnderecoId { get; set; }

        public Endereco Endereco { get; set; }

        public decimal? Total { get; set; }
        public decimal Frete { get; set; }

        public int MelhorEnvioServicoId { get; set; }

        public string? Transportadora { get; set; }

        public string? ServicoFrete { get; set; }

        public decimal Subtotal { get; set; }

        public List<PedidoItem> Itens { get; set; } = new();

        public StatusPedido Status { get; set; }
            = StatusPedido.AguardandoPagamento;

        public MetodoPagamento MetodoPagamento { get; set; }

        public DateTime? DataPagamento { get; set; }

        public string? IdPagamento { get; set; }

        public string? QrCode { get; set; }

        public string? QrCodeBase64 { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public string? CustomerId { get; set; }
        public string? BoletoUrl { get; set; }

        public string? LinhaDigitavel { get; set; }

        public string? NossoNumero { get; set; }
    }
}