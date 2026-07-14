using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;

namespace SiteAspas.Pages.Conta
{
    public class ConfirmarEmailModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;

        public bool Sucesso { get; set; }

        public ConfirmarEmailModel(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (user.EmailConfirmationToken == code && user.TokenExpiration > DateTime.UtcNow)
            {
                user.IsAtivo = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                Sucesso = true;
            }
            else
            {
                Sucesso = false;
            }

            return Page();
        }
    }
}
