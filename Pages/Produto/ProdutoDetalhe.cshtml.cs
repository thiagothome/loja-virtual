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
public class ProdutoDetalheModel : PageModel{
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
    var produto = await _produtoService.ObterPorId(id);

    if (produto == null)
        return NotFound();

    var clienteId = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var itemExistente = await _context.CarrinhoItems
        .FirstOrDefaultAsync(x =>
            x.ClienteId == clienteId &&
            x.ProdutoId == produto.Id);

    // Produto já existe no carrinho
    if (itemExistente != null)
    {
        if (produto.Estoque.HasValue &&
            itemExistente.Quantidade < produto.Estoque.Value)
        {
            itemExistente.Quantidade++;
        }
    }
    else
    {
        // Sem estoque
        if (!produto.Estoque.HasValue ||
            produto.Estoque.Value <= 0)
        {
            TempData["Erro"] =
                "Produto sem estoque.";

            return RedirectToPage(
                "/Produto/ProdutoDetalhe",
                new { id });
        }

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
