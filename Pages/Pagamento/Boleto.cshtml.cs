using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using SiteAspas.Services;

namespace SiteAspas.Pages.Pagamento;

public class BoletoModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly AsaasService _asaasService;
    
    public BoletoModel(
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

        // Criar cliente no Asaas se não tiver
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

        // Criar cobrança de boleto se não tiver
        if (string.IsNullOrEmpty(Pedido.IdPagamento))
        {
            var paymentId =
                await _asaasService.CriarCobrancaBoletoAsync(
                    Pedido.Usuario.CustomerId!,
                    Pedido.Total ?? 0);

            Pedido.IdPagamento = paymentId;
            Pedido.ExpirationDate = DateTime.UtcNow.AddDays(3);
            await _context.SaveChangesAsync();
        }

        // Obter dados do boleto se não tiver
        if (string.IsNullOrEmpty(Pedido.BoletoUrl) && !string.IsNullOrEmpty(Pedido.IdPagamento))
        {
            var boleto = await _asaasService.ObterBoletoAsync(Pedido.IdPagamento);

            if (boleto != null)
            {
                Pedido.BoletoUrl = boleto.BankSlipUrl;
                Pedido.NossoNumero = boleto.NossoNumero;
                await _context.SaveChangesAsync();
            }

            // Obter linha digitável
            var linhaDigitavel = await _asaasService.ObterLinhaDigitavelAsync(Pedido.IdPagamento);
            if (linhaDigitavel != null)
            {
                Pedido.LinhaDigitavel = linhaDigitavel;
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