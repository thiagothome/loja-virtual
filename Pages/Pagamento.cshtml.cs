using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiteAspas.Data;
using SiteAspas.Models;
using Microsoft.EntityFrameworkCore;

public class PagamentoModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;

    public PagamentoModel(
        SiteAspasContext context,
        UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public int PedidoId { get; set; }

    public decimal Total { get; set; }
    public Pedido Pedido { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Entrar");
        }

        Pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == PedidoId && p.UsuarioId == user.Id);

        if (Pedido == null)
        {
            return RedirectToPage("/Carrinho");
        }

        Total = Pedido.Total;
        return Page();
    }

    public async Task<IActionResult> OnPostProcessarPagamentoAsync(string metodo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Entrar");
        }

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == PedidoId && p.UsuarioId == user.Id);

        if (pedido == null)
        {
            return NotFound();
        }

        pedido.Status = "Pagamento Aprovado";
        pedido.MetodoPagamento = metodo;
        pedido.DataPagamento = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToPage("/CompraFinalizada", new { id = pedido.Id });
    }
}