using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas;
using SiteAspas.Data;
using SiteAspas.Models;

public class CarrinhoModel : PageModel
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CarrinhoService _carrinhoService;
    private readonly SiteAspasContext _context;
    private readonly IPedidoService _pedidoService; 

    public CarrinhoModel(CarrinhoService carrinhoService,
                       IHttpContextAccessor httpContextAccessor,
                       SiteAspasContext context,
                       IPedidoService pedidoService) 
    {
        _carrinhoService = carrinhoService;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _pedidoService = pedidoService; 
    }

    public List<CarrinhoItem> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(i => i.Preco * i.Quantidade);

    public void OnGet()
    {
        Itens = _carrinhoService.ObterCarrinho();
    }

    public IActionResult OnPostAtualizarQuantidade(int id, string acao)
    {
        var carrinho = _carrinhoService.ObterCarrinho();
        var item = carrinho.FirstOrDefault(i => i.ProdutoId == id);

        if (item != null)
        {
            if (acao == "aumentar")
            {
                item.Quantidade++;
            }
            else if (acao == "diminuir")
            {
                item.Quantidade--;
                if (item.Quantidade <= 0)
                {
                    carrinho.Remove(item);
                }
            }

            _carrinhoService.SalvarCarrinho(carrinho);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostRemoverItem(int id)
    {
        var carrinho = _carrinhoService.ObterCarrinho();
        var item = carrinho.FirstOrDefault(i => i.ProdutoId == id);

        if (item != null)
        {
            carrinho.Remove(item);
            _carrinhoService.SalvarCarrinho(carrinho);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostFinalizarCompraAsync()
    {
        int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

        if (usuarioId == null)
            return RedirectToPage("/Entrar");

        // Busca o carrinho da sessão
        var carrinho = _carrinhoService.ObterCarrinho();

        if (!carrinho.Any())
        {
            ModelState.AddModelError(string.Empty, "Seu carrinho está vazio.");
            return Page();
        }

        // Buscar os produtos no banco para obter o nome atualizado
        var produtoIds = carrinho.Select(c => c.ProdutoId).ToList();
        var produtos = await _context.Produtos
            .Where(p => produtoIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var pedido = new Pedido
        {
            UsuarioId = usuarioId.Value,
            DataPedido = DateTime.Now,
            Status = "Processando",
            Total = carrinho.Sum(item => item.Preco * item.Quantidade),
            Itens = carrinho.Select(item => new PedidoItem
            {
                ProdutoId = item.ProdutoId,
                Nome = produtos.ContainsKey(item.ProdutoId) ? produtos[item.ProdutoId].Nome : "Produto",
                PrecoUnitario = item.Preco,
                Quantidade = item.Quantidade
            }).ToList()
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // Limpar carrinho da sessão
        _carrinhoService.LimparCarrinho();

        return RedirectToPage("/CompraFinalizada", new { id = pedido.Id });
    }
}
