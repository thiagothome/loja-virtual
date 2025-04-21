using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Services;

[Authorize]
public class ProdutoModel : PageModel
    {
        private readonly CarrinhoService _carrinhoService;
        private readonly ProdutoService _produtoService;

        public ProdutoModel(ProdutoService produtoService, CarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
            _produtoService = produtoService;
        }

        public Produto Produto { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

        if (id == null)
            return RedirectToPage("/Entrar");

        Produto = await _produtoService.ObterPorId(id.Value);

        if (Produto == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(int id)
        {
            var produto = await _produtoService.ObterPorId(id);
            if (produto == null) return NotFound();

            var item = new CarrinhoItem
            {
                ProdutoId = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                ImagemUrl = produto.ImagemUrl,
                Quantidade = 1
            };

            _carrinhoService.AdicionarItem(item); 

            return RedirectToPage("/Carrinho");
        }
    }
