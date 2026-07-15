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
    private readonly IPedidoService _pedidoService;

    public PixModel(
    SiteAspasContext context,
    AsaasService asaasService,
    IPedidoService pedidoService)
    {
        _context = context;
        _asaasService = asaasService;
        _pedidoService = pedidoService;
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

            // CustomerId inválido ou removido
            if (paymentId == null)
            {
                Pedido.Usuario.CustomerId = null;
                await _context.SaveChangesAsync();

                var novoCustomer =
                    await _asaasService.CriarClienteAsync(
                        $"{Pedido.Usuario.Nome} {Pedido.Usuario.Sobrenome}",
                        Pedido.Usuario.Email!,
                        Pedido.Usuario.Telefone ?? "",
                        Pedido.Usuario.CPF ?? "");

                if (string.IsNullOrEmpty(novoCustomer))
                {
                    TempData["Erro"] = "Não foi possível criar o cliente no Asaas.";
                    return Page();
                }

                Pedido.Usuario.CustomerId = novoCustomer;
                await _context.SaveChangesAsync();

                paymentId =
                    await _asaasService.CriarCobrancaPixAsync(
                        novoCustomer,
                        Pedido.Total ?? 0);
            }

            // Ainda falhou? Não salva o pedido.
            if (string.IsNullOrEmpty(paymentId))
            {
                TempData["Erro"] = "Não foi possível gerar o pagamento.";
                return Page();
            }

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
            var confirmado =
                await _pedidoService.ConfirmarPagamento(
                    pedido.Id);

            if (confirmado)
            {
                StatusMessage =
                    "✅ Pagamento aprovado com sucesso!";
            }
            else
            {
                StatusMessage =
                    "❌ Estoque insuficiente para concluir o pedido.";
            }
        }

        return RedirectToPage(new { id });
    }
}