using SiteAspas.Data;
using SiteAspas.Models;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Models.Enums;

namespace SiteAspas.Services
{
public class PedidoService : IPedidoService
{
   private readonly SiteAspasContext _context;

        public PedidoService(
            SiteAspasContext context)
        {
            _context = context;
        }

    public async Task<int> CriarPedido(
    int usuarioId,
    int enderecoId,
    MetodoPagamento metodoPagamento,
    List<CarrinhoItem> itens)
    {
        var pedido = new Pedido
        {
            UsuarioId = usuarioId,
            EnderecoId = enderecoId,
            MetodoPagamento = metodoPagamento,
            Status = StatusPedido.AguardandoPagamento,
            DataPedido = DateTime.UtcNow,
            Total = itens.Sum(i => i.Preco * i.Quantidade),
            Itens = itens.Select(i => new PedidoItem
            {
                ProdutoId = i.ProdutoId,
                Nome = i.Nome,
                PrecoUnitario = i.Preco,
                Quantidade = i.Quantidade
            }).ToList()
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return pedido.Id;
    }

    public async Task<Pedido?> ObterPedidoPorId(int id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
}