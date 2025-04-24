using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using SiteAspas.Services;

namespace SiteAspas.Pages
{
    public class EmailConfirmacaoModel : PageModel
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<Usuario> _userManager;

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public EmailConfirmacaoModel(IEmailService emailService, UserManager<Usuario> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
{
    if (!string.IsNullOrEmpty(Email))
    {
        // Verificando se o usuário está autenticado
        if (User.Identity.IsAuthenticated)
        {
            var novoToken = Guid.NewGuid().ToString();

            // Verificando se o 'User' contém o nome do usuário
            string userName = User.Identity.Name;
            string userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                // Log para verificar se o 'userId' está sendo obtido corretamente
                Console.WriteLine($"UserId: {userId}, UserName: {userName}");
            }

            await _emailService.EnviarEmailConfirmacaoAsync(userId, Email, "Usuário", novoToken);
        }
        else
        {
            // Redirecionar para a página de login, por exemplo
            return RedirectToPage("/Login");
        }
    }
    return Page();
}
    }
}