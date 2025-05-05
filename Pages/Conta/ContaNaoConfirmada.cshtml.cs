using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Services;

namespace SiteAspas.Pages.Conta
{
    public class ContaNaoConfirmadaModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public string Email { get; set; }

        public ContaNaoConfirmadaModel(
            UserManager<Usuario> userManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public void OnGet(string email)
        {
            Email = email;
        }


        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError(string.Empty, "E-mail não informado.");
                return Page();
            }

            var usuario = await _userManager.FindByEmailAsync(Email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return Page();
            }

            if (await _userManager.IsEmailConfirmedAsync(usuario))
            {
                return RedirectToPage("/Conta/Entrar");
            }

            try
            {
                var token = GenerateToken();

                await _emailService.EnviarEmailConfirmacaoAsync(
                    usuario.Id.ToString(),
                    usuario.Email,
                    usuario.Nome,
                    token);

                return RedirectToPage("/Conta/EmailConfirmacao", new { email = Email });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocorreu um erro: {ex.Message}");
                return Page();
            }
        }
        private string GenerateToken()
        {
            return Guid.NewGuid().ToString("N") +
                   Guid.NewGuid().ToString("N").Substring(0, 16);
        }
    }
}