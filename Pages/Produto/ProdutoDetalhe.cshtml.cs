using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Services;
using System.Security.Claims;

namespace SiteAspas.Pages;

[Authorize]
public class ProdutoDetalheModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly ProdutoService _produtoService;

    public ProdutoDetalheModel(
        ProdutoService produtoService,
        SiteAspasContext context)
    {
        _produtoService = produtoService;
        _context = context;
    }

    public SiteAspas.Models.Produto Produto { get; set; }
    public async Task<IActionResult> OnGetAsync(int? id)
    {


        if (id == null)
            return BadRequest("ID do produto � obrigat�rio.");

        Produto = await _produtoService.ObterPorId(id.Value);

        if (Produto == null)
            return NotFound();

        return Page();
    }


    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(int id)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto == null)
            return NotFound();

        // Produto sem estoque
        if (!produto.Estoque.HasValue || produto.Estoque.Value <= 0)
        {
            TempData["Erro"] =
                "Este produto está esgotado no momento.";

            return RedirectToPage("/Produto/ProdutoDetalhe", new { id });
        }

        var clienteId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var itemExistente = await _context.CarrinhoItems
            .FirstOrDefaultAsync(c =>
                c.ClienteId == clienteId &&
                c.ProdutoId == produto.Id);

        // Já atingiu o limite do estoque
        if (itemExistente != null &&
            itemExistente.Quantidade >= produto.Estoque.Value)
        {
            TempData["Erro"] =
                "Você já adicionou a quantidade máxima disponível deste produto.";

            return RedirectToPage("/Produto/ProdutoDetalhe", new { id });
        }

        if (itemExistente != null)
        {
            itemExistente.Quantidade++;
        }
        else
        {
            _context.CarrinhoItems.Add(new CarrinhoItem
            {
                ProdutoId = produto.Id,
                ClienteId = clienteId,
                Nome = produto.Nome,
                ImagemUrl = produto.ImagemUrl,
                Preco = produto.Preco,
                Quantidade = 1
            });
        }

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Produto adicionado ao carrinho.";

        return RedirectToPage("/Produto/Carrinho");
    }


    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostDesativarAsync(int id)
    {
        var produto = await _produtoService.ObterPorId(id);
        if (produto == null) return NotFound();

        produto.Ativo = false;
        await _produtoService.Atualizar(produto);

        return RedirectToPage("/Home/Index");
    }
}
