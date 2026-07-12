using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using System.ComponentModel.DataAnnotations;

namespace SiteAspas.Pages
{
    public class EntrarModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        public EntrarModel(
            SignInManager<Usuario> signInManager,
            UserManager<Usuario> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [BindProperty]
        public bool LembrarMe { get; set; }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuario = await _userManager.FindByEmailAsync(Email);
            if (usuario?.Email != Email)
            {
                ModelState.AddModelError("Email", "Email inválido.");
                return Page();
            }

            if (!usuario.IsAtivo)
            {
                ModelState.AddModelError(string.Empty, "Sua conta ainda não está ativa.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                Email,
                Senha,
                LembrarMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("Senha", "Senha inválida.");
                return Page();
            }

            return RedirectToPage("/Home/Index");
        }
    }
}