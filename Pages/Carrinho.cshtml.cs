using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        Itens = _carrinhoService.ObterCarrinho();
        
        if (!Itens.Any())
        {
            return RedirectToPage("/Carrinho");
        }

        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext não disponível");
        
        string clienteId = httpContext.Request.Cookies["cliente_temp"] 
                        ?? Guid.NewGuid().ToString();

        var pedidoId = await _pedidoService.CriarPedido(clienteId, Itens);
        await _carrinhoService.LimparCarrinho(); // Adicione await aqui
        
        if (!httpContext.Request.Cookies.ContainsKey("cliente_temp"))
        {
            httpContext.Response.Cookies.Append("cliente_temp", clienteId, 
                new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        }
        
        return RedirectToPage("/CompraFinalizada", new { id = pedidoId });
    }
}
