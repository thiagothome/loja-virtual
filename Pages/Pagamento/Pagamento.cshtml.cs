using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiteAspas.Data;
using SiteAspas.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SiteAspas.Pages;

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

    public decimal? Total { get; set; }
    public Pedido Pedido { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Conta/Entrar");
        }

        Pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == PedidoId && p.UsuarioId == user.Id);

        if (Pedido == null)
        {
            return RedirectToPage("/Produto/Carrinho");
        }

        Total = Pedido.Total;
        return Page();
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostProcessarPagamentoAsync(
      string metodo,
      [FromForm] string numeroCartao,
      [FromForm] string validade,
      [FromForm] string cvv,
      [FromForm] string nomeTitular,
      [FromForm] int parcelas)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Conta/Entrar");

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == PedidoId && p.UsuarioId == user.Id);
        if (pedido == null) return NotFound();

        if (metodo == "pix")
        {
            try
            {
                var pixResult = await _mercadoPagoService.CriarPagamentoPixAsync(pedido);


                pedido.Status = "Aguardando Pagamento";
                pedido.MetodoPagamento = "PIX";
                pedido.IdPagamento = pixResult.Id;
                pedido.QrCode = pixResult.QrCode;
                pedido.QrCodeBase64 = pixResult.QrCodeBase64;
                pedido.ExpirationDate = pixResult.ExpirationDate;

                await _context.SaveChangesAsync();


                return RedirectToPage("/Pagamento/PagamentoPix", new { pedidoId = pedido.Id });
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

                    return RedirectToPage("/Pagamento/CompraFinalizada", new { id = pedido.Id });
                }
                else
                {
                    var errorMessage = paymentResult.Status switch
                    {
                        "rejected" => "Pagamento recusado",
                        "in_process" => "Pagamento em processamento",
                        "pending" => "Pagamento pendente",
                        "cancelled" => "Pagamento cancelado",
                        "refunded" => "Pagamento reembolsado",
                        "charged_back" => "Estorno no cartão",
                        _ => $"Status de pagamento não reconhecido: {paymentResult.Status}"
                    };


                    if (!string.IsNullOrEmpty(paymentResult.StatusDetail))
                    {
                        errorMessage += $". Detalhe: {paymentResult.StatusDetail}";
                    }

                    if (!string.IsNullOrEmpty(paymentResult.Error))
                    {
                        errorMessage += $". Erro: {paymentResult.Error}";
                    }

                    ModelState.AddModelError(string.Empty, errorMessage);
                    _logger.LogWarning("Pagamento não aprovado para pedido {PedidoId}. Status: {Status}, Detalhe: {Detail}",
                        pedido.Id, paymentResult.Status, paymentResult.StatusDetail);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao processar pagamento com cartão: {ex.Message}");
                _logger.LogError(ex, "Erro no processamento do cartão para pedido {PedidoId}", pedido.Id);
            }
        }


        Pedido = pedido;
        Total = pedido.Total;
        return Page();
    }
}