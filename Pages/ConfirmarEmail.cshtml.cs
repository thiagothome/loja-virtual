using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;

namespace SiteAspas.Pages
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

            var result = await _userManager.ConfirmEmailAsync(user, code);
            Sucesso = result.Succeeded;

            return Page();
        }
    }
}
