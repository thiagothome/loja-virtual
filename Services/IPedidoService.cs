using SiteAspas;
using SiteAspas.Models;

public interface IPedidoService
{
    Task<int> CriarPedido(string usuarioId, List<CarrinhoItem> itens);
    Task<Pedido?> ObterPedidoPorId(int id); 
}
