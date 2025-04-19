using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Data;
using Microsoft.AspNetCore.Authorization;

namespace SiteAspas.Pages
{
    public class CadastrarProdutoModel : PageModel
    {
        private readonly SiteAspasContext _context;
        private readonly IWebHostEnvironment _environment;

        public CadastrarProdutoModel(SiteAspasContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Produto Produto { get; set; }

        [BindProperty]
        public IFormFile ImagemProduto { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Processar upload da imagem
            if (ImagemProduto != null && ImagemProduto.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "produtos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImagemProduto.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagemProduto.CopyToAsync(fileStream);
                }

                Produto.ImagemUrl = "/img/produtos/" + uniqueFileName;
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Produto", new { id = Produto.Id });
        }
    }
}