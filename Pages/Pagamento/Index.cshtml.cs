using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using SiteAspas.Services;

namespace SiteAspas.Pages.Pagamento;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly IPedidoService _pedidoService;

    public IndexModel(
        SiteAspasContext context,
        UserManager<Usuario> userManager,
        IPedidoService pedidoService)
    {
        _context = context;
        _userManager = userManager;
        _pedidoService = pedidoService;
    }

    public List<CarrinhoItem> Itens { get; set; } = new();
    public Endereco? EnderecoPrincipal { get; set; }
    public decimal Total { get; set; }

    public async Task OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return;

        Itens = await _context.CarrinhoItems
            .Where(c => c.ClienteId == usuario.Id)
            .ToListAsync();

        EnderecoPrincipal = await _context.Enderecos
            .FirstOrDefaultAsync(e =>
                e.UsuarioId == usuario.Id &&
                e.Principal);

        Total = Itens.Sum(x =>
            (x.Preco ?? 0) * x.Quantidade);
    }

    public async Task<IActionResult> OnPostAsync(string FormaPagamento)
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return Challenge();

        var itens = await _context.CarrinhoItems
            .Where(c => c.ClienteId == usuario.Id)
            .ToListAsync();

        if (!itens.Any())
        {
            return RedirectToPage("/Produto/Carrinho");
        }

        var endereco = await _context.Enderecos
            .FirstOrDefaultAsync(e =>
                e.UsuarioId == usuario.Id &&
                e.Principal);

        if (endereco == null)
        {
            return RedirectToPage(
                "/Conta/CadastrarUsuarioCompleto");
        }

        MetodoPagamento metodo;
        string paginaRedirecionamento;

        if (FormaPagamento == "Pix")
        {
            metodo = MetodoPagamento.Pix;
            paginaRedirecionamento = "/Pagamento/Pix";
        }
        else if (FormaPagamento == "CartaoCredito")
        {
            metodo = MetodoPagamento.CartaoCredito;
            paginaRedirecionamento = "/Pagamento/Cartao";
        }
        else if (FormaPagamento == "Boleto")
        {
            metodo = MetodoPagamento.Boleto;
            paginaRedirecionamento = "/Pagamento/Boleto";
        }
        else
        {
            ModelState.AddModelError("", "Forma de pagamento inválida");
            return Page();
        }

        var pedidoId = await _pedidoService.CriarPedido(
            usuario.Id,
            endereco.Id,
            metodo,
            itens
        );

        return RedirectToPage(
            paginaRedirecionamento,
            new { id = pedidoId });
    }
}