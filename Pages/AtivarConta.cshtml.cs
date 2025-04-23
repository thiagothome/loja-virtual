using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SiteAspas.Pages
{
    [Authorize]
    public class AtivarContaModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }

        public AtivarContaModel(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
            {
                Mensagem = "Token ou e-mail inválido";
                return Page();
            }

            var normalizedEmail = _userManager.NormalizeEmail(Email);
            var user = await _userManager.FindByEmailAsync(normalizedEmail);
            if (user == null)
            {
                Mensagem = "Usuário não encontrado";
                return Page();
            }

            if (user.EmailConfirmationToken != Token ||
                user.TokenExpiration < DateTime.UtcNow)
            {
                Mensagem = "Token inválido ou expirado";
                return Page();
            }

            user.IsAtivo = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            Sucesso = true;
            Mensagem = "Conta ativada com sucesso!";
            return Page();
        }
    }
}