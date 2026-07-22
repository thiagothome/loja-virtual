using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using SiteAspas.Services;
using SiteAspas.Models.MelhorEnvio;
using System.Globalization;



namespace SiteAspas.Pages.Pagamento;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly IPedidoService _pedidoService;
    private readonly MelhorEnvioService _melhorEnvioService;
    public List<CotacaoFreteResponse> OpcoesFrete { get; set; } = new();
    public decimal FreteSelecionado { get; set; }

    public decimal TotalGeral =>
        Total + FreteSelecionado;

    public IndexModel(
    SiteAspasContext context,
    UserManager<Usuario> userManager,
    IPedidoService pedidoService,
    MelhorEnvioService melhorEnvioService)
    {
        _context = context;
        _userManager = userManager;
        _pedidoService = pedidoService;
        _melhorEnvioService = melhorEnvioService;
    }

    public List<CarrinhoItem> Itens { get; set; } = new();
    public Endereco? EnderecoPrincipal { get; set; }
    public decimal Total { get; set; }

    public async Task OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return;

        Itens = await _context.CarrinhoItems
            .Where(c => c.ClienteId == usuario.Id)
            .ToListAsync();

        EnderecoPrincipal = await _context.Enderecos
            .FirstOrDefaultAsync(e =>
                e.UsuarioId == usuario.Id &&
                e.Principal);

        Total = Itens.Sum(x =>
            (x.Preco ?? 0) * x.Quantidade);

        if (EnderecoPrincipal != null)
        {
            var cotacao = new CotacaoFreteRequest
            {
                CepOrigem = "98803360",
                CepDestino = EnderecoPrincipal.CEP,

                Produtos = Itens.Select(i => new ProdutoFreteRequest
                {
                    Id = i.ProdutoId.ToString(),

                    Altura = i.Altura ?? 10,
                    Largura = i.Largura ?? 10,
                    Comprimento = i.Comprimento ?? 10,

                    Peso = i.Peso ?? 0.5m,

                    Quantidade = i.Quantidade,

                    ValorDeclarado = i.Preco ?? 0
                }).ToList()
            };


            OpcoesFrete =
                await _melhorEnvioService.CalcularFreteAsync(cotacao);
        }
    }

    public async Task<IActionResult> OnPostAsync(
    string FormaPagamento,
    int ServicoId,
    string FreteSelecionado,
    string Transportadora,
    string ServicoFrete)
    {

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return Challenge();

        var itens = await _context.CarrinhoItems
            .Where(c => c.ClienteId == usuario.Id)
            .ToListAsync();

        if (!itens.Any())
        {
            return RedirectToPage("/Produto/Carrinho");
        }

        // Validar estoque antes de criar pedido
        foreach (var item in itens)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == item.ProdutoId);

            if (produto == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Produto {item.Nome} não encontrado.");

                await OnGetAsync();
                return Page();
            }

            if (item.Quantidade > produto.Estoque)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"O produto {item.Nome} possui apenas {produto.Estoque} unidade(s) em estoque.");

                await OnGetAsync();
                return Page();
            }
        }

        var endereco = await _context.Enderecos
            .FirstOrDefaultAsync(e =>
                e.UsuarioId == usuario.Id &&
                e.Principal);

        if (endereco == null)
        {
            return RedirectToPage(
                "/Conta/CadastrarUsuarioCompleto");
        }

        MetodoPagamento metodo;
        string paginaRedirecionamento;

        if (FormaPagamento == "Pix")
        {
            metodo = MetodoPagamento.Pix;
            paginaRedirecionamento = "/Pagamento/Pix";
        }
        else if (FormaPagamento == "CartaoCredito")
        {
            metodo = MetodoPagamento.CartaoCredito;
            paginaRedirecionamento = "/Pagamento/Cartao";
        }
        else if (FormaPagamento == "Boleto")
        {
            metodo = MetodoPagamento.Boleto;
            paginaRedirecionamento = "/Pagamento/Boleto";
        }
        else
        {
            ModelState.AddModelError("", "Forma de pagamento inválida");
            return Page();
        }


decimal frete = decimal.Parse(
    FreteSelecionado,
    CultureInfo.InvariantCulture);

        var pedidoId =
    await _pedidoService.CriarPedido(
        usuario.Id,
        endereco.Id,
        metodo,
        itens,
        frete,
        ServicoId,
        Transportadora,
        ServicoFrete);

        return RedirectToPage(
            paginaRedirecionamento,
            new { id = pedidoId });
    }
}