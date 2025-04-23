using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiteAspas.Data;

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

    public decimal Valor { get; set; }
    public string QrCode { get; set; }
    public string QrCodeBase64 { get; set; }
    public string CodigoPix { get; set; }

    public async Task OnGetAsync(string qrCode, string qrCodeBase64, decimal valor)
    {
        var pedido = await _context.Pedidos.FindAsync(PedidoId);
        if (pedido == null)
        {
            RedirectToPage("/Carrinho");
            return;
        }

        Valor = valor;
        QrCode = qrCode;
        QrCodeBase64 = qrCodeBase64;
        CodigoPix = QrCode;
    }

    public async Task<IActionResult> OnPostVerificarPagamentoAsync()
    {
        try
        {
            var pedido = await _context.Pedidos.FindAsync(PedidoId);
            if (pedido == null)
            {
                return NotFound();
            }

            var status = await _mercadoPagoService.ObterStatusPagamentoAsync(pedido.IdPagamento);

            if (status == "approved")
            {
                pedido.Status = "Aprovado";
                pedido.DataPagamento = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToPage("/CompraFinalizada", new { id = PedidoId });
            }

            TempData["Mensagem"] = $"Pagamento ainda não confirmado. Status: {status}";
            return RedirectToPage(new { pedidoId = PedidoId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar pagamento PIX");
            TempData["Erro"] = "Erro ao verificar pagamento";
            return RedirectToPage(new { pedidoId = PedidoId });
        }
    }

    public IActionResult OnPostCancelar()
    {
        return RedirectToPage("/Pagamento", new { pedidoId = PedidoId });
    }
}