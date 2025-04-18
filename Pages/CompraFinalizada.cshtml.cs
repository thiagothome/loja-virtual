using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
public class CompraFinalizadaModel : PageModel
{
    private readonly IPedidoService _pedidoService;
    
    public Pedido Pedido { get; set; }
    
    public CompraFinalizadaModel(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }
    
    public async Task OnGetAsync(int id)
    {
        Pedido = await _pedidoService.ObterPedidoPorId(id);
        
        if (Pedido == null)
        {
            RedirectToPage("/Index");
        }
    }
}