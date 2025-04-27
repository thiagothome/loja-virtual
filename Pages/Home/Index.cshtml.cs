using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Services;

namespace SiteAspas.Pages;

public class IndexModel : PageModel
{
    private readonly ProdutoService _produtoService;
    private readonly UserManager<Usuario> _userManager;
    public List<Produto> ProdutosDestaque { get; set; } = new();

    public IndexModel(ProdutoService produtoService, UserManager<Usuario> userManager)
    {
        _produtoService = produtoService;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        ProdutosDestaque = await _produtoService.ObterDestaques();
    }

    public async Task<IActionResult> OnPostDesativarAsync(int id)
    {
        var produto = await _produtoService.ObterPorId(id);
        if (produto == null)
        {
            return NotFound();
        }

        produto.Ativo = false;
        await _produtoService.Atualizar(produto);

        return RedirectToPage("/Home/Index");
    }
}
