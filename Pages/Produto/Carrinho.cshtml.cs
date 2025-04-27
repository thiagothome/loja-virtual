using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Data;
using SiteAspas.Models;

[Authorize]
public class CarrinhoModel : PageModel
{
    private readonly CarrinhoService _carrinhoService;
    private readonly SiteAspasContext _context;
    private readonly IPedidoService _pedidoService;
    private readonly UserManager<Usuario> _userManager;

    public CarrinhoModel(
        CarrinhoService carrinhoService,
        SiteAspasContext context,
        IPedidoService pedidoService,
        UserManager<Usuario> userManager)
    {
        _carrinhoService = carrinhoService;
        _context = context;
        _pedidoService = pedidoService;
        _userManager = userManager;
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
        var user = await _userManager.GetUserAsync(User);


        if (!user.CadastroCompleto)
        {

            return RedirectToPage("/Conta/CadastrarUsuarioCompleto", new { id = user.Id, returnUrl = "/Produto/Carrinho" });
        }

        if (user == null)
        {
            return RedirectToPage("/Conta/Entrar");
        }

        var carrinho = _carrinhoService.ObterCarrinho();

        if (!carrinho.Any())
        {
            ModelState.AddModelError(string.Empty, "Seu carrinho está vazio.");
            return Page();
        }

        /* foreach (var item in carrinho)
         {
             var produto = await _context.Produtos.FindAsync(item.ProdutoId);
             if (produto == null || produto.QuantidadeEstoque < item.Quantidade)
             {
                 ModelState.AddModelError(string.Empty,
                     $"Produto {produto?.Nome ?? "não encontrado"} sem estoque suficiente");
                 return Page();
             }
         }*/

        var pedido = new Pedido
        {
            UsuarioId = user.Id,
            DataPedido = DateTime.Now,
            Status = "Processando",
            Total = carrinho.Sum(item => item.Preco * item.Quantidade),
            Itens = new List<PedidoItem>()
        };

        foreach (var item in carrinho)
        {
            var produto = await _context.Produtos.FindAsync(item.ProdutoId);

            pedido.Itens.Add(new PedidoItem
            {
                ProdutoId = item.ProdutoId,
                Nome = produto?.Nome ?? "Produto não encontrado",
                PrecoUnitario = item.Preco,
                Quantidade = item.Quantidade
            });

            /* 
             if (produto != null)
             {
                 produto.QuantidadeEstoque -= item.Quantidade;
             }*/
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = RedirectToPage("/Pagamento/Pagamento", new { pedidoId = pedido.Id });

                _carrinhoService.LimparCarrinho();

                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao processar seu pedido.");
                return Page();
            }
        }
    }
}