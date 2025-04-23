using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Services;

namespace SiteAspas.Pages
{
    [Authorize]
    public class EmailConfirmacaoModel : PageModel
    {
        private readonly IEmailService _emailService;

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public EmailConfirmacaoModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Email))
            {
                var novoToken = Guid.NewGuid().ToString();
                await _emailService.EnviarEmailConfirmacaoAsync(Email, "Usuário", novoToken);
            }
            return Page();
        }
    }
}