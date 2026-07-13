using SiteAspas.Models;
using SiteAspas.Models.Enums;

namespace SiteAspas.Services
{
    public interface IPedidoService
    {
        Task<int> CriarPedido(
            int usuarioId,
            int enderecoId,
            MetodoPagamento metodoPagamento,
            List<CarrinhoItem> itens
        );


        Task<Pedido?> ObterPedidoPorId(
            int id
        );
        Task<bool> ConfirmarPagamento(int pedidoId);
    }
}