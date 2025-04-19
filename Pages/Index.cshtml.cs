using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Services;

namespace SiteAspas.Pages;

public class IndexModel : PageModel
{
    private readonly ProdutoService _produtoService;

    public List<Produto> ProdutosDestaque { get; set; } = new();

    public IndexModel(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

      public async Task OnGetAsync()
    {
        ProdutosDestaque = await _produtoService.ObterDestaques();
    }
}
