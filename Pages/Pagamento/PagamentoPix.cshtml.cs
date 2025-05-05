using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;

namespace SiteAspas.Pages;

public class PagamentoPixModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly MercadoPagoService _mercadoPagoService;
    private readonly ILogger<PagamentoPixModel> _logger;

    public PagamentoPixModel(
        SiteAspasContext context,
        MercadoPagoService mercadoPagoService,
        ILogger<PagamentoPixModel> logger)
    {
        _context = context;
        _mercadoPagoService = mercadoPagoService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int PedidoId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string QrCode { get; set; }

    [BindProperty(SupportsGet = true)]
    public string QrCodeBase64 { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? Valor { get; set; }
    public DateTime ExpirationDate { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {

        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == PedidoId);

        if (pedido == null || pedido.Status != "Aguardando Pagamento")
        {
            return RedirectToPage("/Home/Index");
        }


        Valor = pedido.Total;
        QrCode = pedido.QrCode;
        QrCodeBase64 = pedido.QrCodeBase64;
        ExpirationDate = pedido.ExpirationDate ?? DateTime.UtcNow.AddMinutes(30);

        return Page();
    }


    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostVerificarPagamentoAsync()
    {
        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == PedidoId);

        if (pedido == null)
        {
            return NotFound();
        }

        try
        {
            var status = await _mercadoPagoService.ObterStatusPagamentoAsync(pedido.IdPagamento);

            if (status == "approved")
            {
                pedido.Status = "Pagamento Aprovado";
                pedido.DataPagamento = DateTime.Now;
                await _context.SaveChangesAsync();

                return RedirectToPage("/Pagamento/CompraFinalizada", new { id = pedido.Id });
            }
            else if (status == "cancelled" || status == "rejected")
            {
                TempData["ErrorMessage"] = "Pagamento não aprovado. Por favor, tente novamente.";
                return RedirectToPage("/Pagamento/Pagamento", new { pedidoId = pedido.Id });
            }


            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar status do pagamento PIX");
            TempData["ErrorMessage"] = "Erro ao verificar pagamento. Tente novamente.";
            return Page();
        }
    }


    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostCancelarAsync()
    {
        var pedido = await _context.Pedidos.FindAsync(PedidoId);
        if (pedido != null)
        {
            pedido.Status = "Cancelado";
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Home/Index");
    }
}