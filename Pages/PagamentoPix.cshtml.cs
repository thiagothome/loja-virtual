using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiteAspas.Data;

public class PagamentoPixModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly ILogger<PagamentoPixModel> _logger;

    public PagamentoPixModel(SiteAspasContext context, ILogger<PagamentoPixModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int PedidoId { get; set; }

    public decimal Valor { get; set; }
    public string CodigoTeste { get; set; } 

    public async Task OnGetAsync()
    {
        
        Valor = decimal.Parse(TempData["ValorPix"]?.ToString() ?? "100.00");
        CodigoTeste = $"PIXTEST{DateTime.Now:HHmmss}";

        
        TempData.Keep("ValorPix");
        TempData.Keep("PedidoId");
    }

    public async Task<IActionResult> OnPostSimularPagamentoAsync()
    {
        try
        {
            var pedido = await _context.Pedidos.FindAsync(PedidoId);
            if (pedido == null)
            {
                ModelState.AddModelError(string.Empty, "Pedido não encontrado");
                return Page();
            }

            
            pedido.Status = "Aprovado";
            pedido.MetodoPagamento = "PIX";
            pedido.DataPagamento = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage("/CompraFinalizada", new { id = PedidoId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao simular pagamento PIX");
            ModelState.AddModelError(string.Empty, "Erro ao processar pagamento");
            return Page();
        }
    }

    public IActionResult OnPostCancelar()
    {
        return RedirectToPage("/Pagamento", new { pedidoId = PedidoId });
    }
}