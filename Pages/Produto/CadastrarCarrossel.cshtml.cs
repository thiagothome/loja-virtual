using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SiteAspas.Pages
{
    public class CadastrarCarrosselModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> Imagens { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Imagens == null || Imagens.Count != 6)
            {
                ModelState.AddModelError("", "Envie exatamente 6 imagens.");
                return Page();
            }

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/carrossel");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            for (int i = 0; i < 6; i++)
            {
                var imagem = Imagens[i];
                if (imagem.Length > 0)
                {
                    var nome = $"v{i + 1}.jpg"; 
                    var caminho = Path.Combine(pasta, nome);

                    using var stream = new FileStream(caminho, FileMode.Create);
                    await imagem.CopyToAsync(stream);
                }
            }

            TempData["Mensagem"] = "Imagens atualizadas com sucesso!";
            return RedirectToPage();
        }
    }
}
