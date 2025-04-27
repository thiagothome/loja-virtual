using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiteAspas.Data;
using SiteAspas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CompraFinalizadaModel : PageModel
{
    private readonly SiteAspasContext _context;

    public CompraFinalizadaModel(SiteAspasContext context)
    {
        _context = context;
    }

    public Pedido Pedido { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Pedido == null)
            return NotFound();

        return Page();
    }
}
