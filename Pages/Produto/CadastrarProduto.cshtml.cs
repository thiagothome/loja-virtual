using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SiteAspas.Pages
{
    [Authorize]
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
        [Required(ErrorMessage = "A imagem do produto é obrigatória.")]
        public IFormFile ImagemProduto { get; set; }

        public void OnGet()
        {
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ImagemProduto != null && ImagemProduto.Length > 0)
            {
                var ext = Path.GetExtension(ImagemProduto.FileName).ToLower();
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (!extensoesPermitidas.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Formato de imagem não permitido.");
                    return Page();
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "produtos");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + ext;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagemProduto.CopyToAsync(fileStream);
                }

                Produto.ImagemUrl = "/img/produtos/" + uniqueFileName;
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Produto/Produto", new { id = Produto.Id });
        }
    }
}