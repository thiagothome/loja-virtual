using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using SiteAspas.Services;

namespace SiteAspas.Pages.Pagamento;

public class CartaoModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly AsaasService _asaasService;
    private readonly IPedidoService _pedidoService;
    
    public CartaoModel(
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

    [BindProperty]
    public string NumeroCartao { get; set; } = string.Empty;

    [BindProperty]
    public string NomeCartao { get; set; } = string.Empty;

    [BindProperty]
    public string MesExpiracao { get; set; } = string.Empty;

    [BindProperty]
    public string AnoExpiracao { get; set; } = string.Empty;

    [BindProperty]
    public string Cvv { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Pedido == null)
            return NotFound();

        if (Pedido.Status == StatusPedido.Pago)
            return Page();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Pedido == null)
            return NotFound();

        // Limpar CPF
        var cpfLimpo = string.IsNullOrEmpty(Pedido.Usuario.CPF)
            ? ""
            : new string(Pedido.Usuario.CPF.Where(char.IsDigit).ToArray());

        // Criar cliente se necessário
        if (string.IsNullOrEmpty(Pedido.Usuario.CustomerId))
        {
            var customerId = await _asaasService.CriarClienteAsync(
                $"{Pedido.Usuario.Nome} {Pedido.Usuario.Sobrenome}",
                Pedido.Usuario.Email!,
                Pedido.Usuario.Telefone ?? "",
                Pedido.Usuario.CPF ?? "");

            if (string.IsNullOrEmpty(customerId))
            {
                StatusMessage = "❌ Erro ao processar pagamento.";
                return Page();
            }

            Pedido.Usuario.CustomerId = customerId;
            await _context.SaveChangesAsync();
        }

        // Processar pagamento
        var resultado = await _asaasService.CriarCobrancaCartaoAsync(
            Pedido.Usuario.CustomerId!,
            Pedido.Total ?? 0,
            NomeCartao,
            NumeroCartao,
            MesExpiracao,
            AnoExpiracao,
            Cvv,
            Pedido.Usuario.Email!,
            cpfLimpo,
            "01001000", // CEP (ideal pegar do endereço)
            "100",      // Número (ideal pegar do endereço)
            Pedido.Usuario.Telefone ?? "");

     if (resultado != null && resultado.Status == "CONFIRMED")
{
    Pedido.IdPagamento = resultado.Id;
    await _context.SaveChangesAsync();

    var confirmado =
        await _pedidoService.ConfirmarPagamento(
            Pedido.Id);

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