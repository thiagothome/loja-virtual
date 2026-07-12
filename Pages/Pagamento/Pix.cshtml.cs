using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using Microsoft.AspNetCore.Identity;
using SiteAspas.Services;

namespace SiteAspas.Pages.Pagamento;

public class PixModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly AsaasService _asaasService;
    
    public PixModel(
        SiteAspasContext context,
        AsaasService asaasService)
    {
        _context = context;
        _asaasService = asaasService;
    }

    public Pedido? Pedido { get; set; }
    
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Pedido == null)
            return NotFound();

        // Se já estiver pago, só mostra a página
        if (Pedido.Status == StatusPedido.Pago)
            return Page();

        if (string.IsNullOrEmpty(Pedido.Usuario.CustomerId))
        {
            var customerId =
                await _asaasService.CriarClienteAsync(
                    $"{Pedido.Usuario.Nome} {Pedido.Usuario.Sobrenome}",
                    Pedido.Usuario.Email!,
                    Pedido.Usuario.Telefone ?? "",
                    Pedido.Usuario.CPF ?? "");

            Pedido.Usuario.CustomerId = customerId;
            await _context.SaveChangesAsync();
        }

        if (string.IsNullOrEmpty(Pedido.IdPagamento))
        {
            var paymentId =
                await _asaasService.CriarCobrancaPixAsync(
                    Pedido.Usuario.CustomerId!,
                    Pedido.Total ?? 0);

            Pedido.IdPagamento = paymentId;
            await _context.SaveChangesAsync();
        }

        if (string.IsNullOrEmpty(Pedido.QrCodeBase64) && !string.IsNullOrEmpty(Pedido.IdPagamento))
        {
            var qr =
                await _asaasService.ObterQrCodePixAsync(
                    Pedido.IdPagamento!);

            if (qr != null)
            {
                Pedido.QrCode = qr.Payload;
                Pedido.QrCodeBase64 = qr.EncodedImage;
                await _context.SaveChangesAsync();
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSimularPagamentoAsync(int id)
{
    var pedido = await _context.Pedidos
        .FirstOrDefaultAsync(p => p.Id == id);

    if (pedido == null || string.IsNullOrEmpty(pedido.IdPagamento))
        return NotFound();

    var sucesso = await _asaasService.SimularPagamentoPixAsync(
        pedido.IdPagamento, 
        pedido.Total ?? 0,
        DateTime.Now);

    if (sucesso)
    {
        pedido.Status = StatusPedido.Pago;
        pedido.DataPagamento = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        StatusMessage = "✅ Pagamento simulado com sucesso!";
    }
    else
    {
        StatusMessage = "❌ Erro ao simular pagamento. Tente novamente.";
    }

    return RedirectToPage(new { id });
}
}