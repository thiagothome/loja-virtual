using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SiteAspas.Models;
using SiteAspas.Services;
using System;

namespace SiteAspas.Pages
{
    public class EmailConfirmacaoModel : PageModel
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<Usuario> _userManager;

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public EmailConfirmacaoModel(
            IEmailService emailService,
            UserManager<Usuario> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToPage("/Conta/Entrar");
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return NotFound();
            }

            user.EmailConfirmationToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N").Substring(0, 16);
            user.TokenExpiration = DateTime.UtcNow.AddHours(24);

            await _userManager.UpdateAsync(user);

            await _emailService.EnviarEmailConfirmacaoAsync(
                user.Id.ToString(),
                user.Email,
                user.Nome,
                user.EmailConfirmationToken);

            return Page();
        }
    }
}