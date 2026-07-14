using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;

namespace SiteAspas.Pages.Conta
{

    [Authorize]
    public class SairModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;

        public SairModel(SignInManager<Usuario> signInManager)
        {
            _signInManager = signInManager;
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Home/Index");
        }
    }
}