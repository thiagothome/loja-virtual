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
    List<CarrinhoItem> itens,
    decimal frete,
    int servicoId,
    string transportadora,
    string servicoFrete)
        {


            var subtotal =
    itens.Sum(i => (i.Preco ?? 0) * i.Quantidade);

            var total =
                subtotal + frete;

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,

                EnderecoId = enderecoId,

                MetodoPagamento = metodoPagamento,

                Status = StatusPedido.AguardandoPagamento,

                DataPedido = DateTime.UtcNow,

                Subtotal = subtotal,

                Frete = frete,

                MelhorEnvioServicoId = servicoId,

                Transportadora = transportadora,

                ServicoFrete = servicoFrete,

                Total = total,

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

        public async Task<bool> ConfirmarPagamento(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                return false;

            foreach (var item in pedido.Itens)
            {
                var produto = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.Id == item.ProdutoId);

                if (produto == null)
                    return false;

                if (produto.Estoque < item.Quantidade)
                    return false;
            }

            foreach (var item in pedido.Itens)
            {
                var produto = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.Id == item.ProdutoId);

                produto!.Estoque -= item.Quantidade;
            }

            pedido.Status = StatusPedido.Pago;
            pedido.DataPagamento = DateTime.UtcNow;

            var carrinho = await _context.CarrinhoItems
                .Where(c => c.ClienteId == pedido.UsuarioId)
                .ToListAsync();

            _context.CarrinhoItems.RemoveRange(carrinho);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}