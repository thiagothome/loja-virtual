using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiteAspas.Data;
using SiteAspas.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class PagamentoModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly MercadoPagoService _mercadoPagoService;
    private readonly ILogger<PagamentoModel> _logger;

    public PagamentoModel(
        SiteAspasContext context,
        UserManager<Usuario> userManager,
        MercadoPagoService mercadoPagoService,
        ILogger<PagamentoModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _mercadoPagoService = mercadoPagoService;
        _logger = logger;
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

  public async Task<IActionResult> OnPostProcessarPagamentoAsync(
    string metodo,
    [FromForm] string numeroCartao,
    [FromForm] string validade,
    [FromForm] string cvv,
    [FromForm] string nomeTitular,
    [FromForm] int parcelas)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return RedirectToPage("/Entrar");

    var pedido = await _context.Pedidos
        .FirstOrDefaultAsync(p => p.Id == PedidoId && p.UsuarioId == user.Id);
    if (pedido == null) return NotFound();

    if (metodo == "pix")
    {
        try
        {
            var pixResponse = await _mercadoPagoService.CriarPagamentoPixAsync(pedido);
            
            using var jsonDoc = JsonDocument.Parse(pixResponse);
            var root = jsonDoc.RootElement;
            
            pedido.Status = "Aguardando Pagamento";
            pedido.MetodoPagamento = "PIX";
            pedido.IdPagamento = root.GetProperty("id").ToString();
            
            var qrCode = root.GetProperty("point_of_interaction")
                             .GetProperty("transaction_data")
                             .GetProperty("qr_code").GetString();
            
            var qrCodeBase64 = root.GetProperty("point_of_interaction")
                                  .GetProperty("transaction_data")
                                  .GetProperty("qr_code_base64").GetString();
            
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/PagamentoPix", new { 
                pedidoId = pedido.Id,
                qrCode = qrCode,
                qrCodeBase64 = qrCodeBase64,
                valor = pedido.Total
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pagamento PIX");
            ModelState.AddModelError(string.Empty, "Erro ao processar pagamento PIX");
            Pedido = pedido;
            Total = pedido.Total;
            return Page();
        }
    }
        else if (metodo == "cartao")
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(numeroCartao) || numeroCartao.Length < 13)
                {
                    ModelState.AddModelError(string.Empty, "Número do cartão inválido");
                    return await OnGetAsync(); 
                }

                
                var paymentResult = await _mercadoPagoService.ProcessarPagamentoCartaoAsync(
                    pedido,
                    numeroCartao,
                    validade,
                    cvv,
                    nomeTitular,
                    parcelas);

                if (paymentResult.Status == "approved")
                {
                    pedido.Status = "Pagamento Aprovado";
                    pedido.MetodoPagamento = $"Cartão de Crédito ({parcelas}x)";
                    pedido.DataPagamento = DateTime.Now;
                    pedido.IdPagamento = paymentResult.Id.ToString();

                    await _context.SaveChangesAsync();

                    return RedirectToPage("/CompraFinalizada", new { id = pedido.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        $"Pagamento não aprovado. Status: {paymentResult.Status}");
                    _logger.LogWarning("Pagamento recusado para pedido {PedidoId}. Status: {Status}",
                        pedido.Id, paymentResult.Status);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao processar pagamento com cartão");
                _logger.LogError(ex, "Erro no processamento do cartão para pedido {PedidoId}", pedido.Id);
            }
        }

        
        Pedido = pedido;
        Total = pedido.Total;
        return Page();
    }
}