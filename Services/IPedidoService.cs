using SiteAspas.Models;

public interface IPedidoService
{
    Task<int> CriarPedido(int usuarioId, List<CarrinhoItem> itens);
    Task<Pedido?> ObterPedidoPorId(int id); 
}
