using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas;
using SiteAspas.Models;
using SiteAspas.Services;

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

        public IActionResult OnGet(int id)
        {
            Produto = _produtoService.ObterPorId(id);

            if (Produto == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPostAdicionarAoCarrinho(int id)
        {
            var produto = _produtoService.ObterPorId(id);
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
