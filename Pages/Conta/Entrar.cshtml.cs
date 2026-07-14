using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SiteAspas.Pages.Conta
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

        public async Task<IActionResult> OnGetCallbackAsync(
            string? returnUrl = null,
            string? remoteError = null)
        {
            Console.WriteLine("CALLBACK GOOGLE EXECUTADO");

            if (remoteError != null)
            {
                Console.WriteLine($"ERRO GOOGLE: {remoteError}");
                return RedirectToPage("/Conta/Entrar");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                Console.WriteLine("INFO NULL");
                return RedirectToPage("/Conta/Entrar");
            }

            Console.WriteLine("INFO OK");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("EMAIL NÃO RETORNADO PELO GOOGLE");
                return RedirectToPage("/Conta/Entrar");
            }

            var usuario = await _userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                Console.WriteLine($"CRIANDO USUÁRIO: {email}");

                var nomes = (name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);

                usuario = new Usuario
                {
                    UserName = email,
                    Email = email,
                    NormalizedEmail = _userManager.NormalizeEmail(email),
                    NormalizedUserName = _userManager.NormalizeName(email),
                    Nome = nomes.Length > 0 ? nomes[0] : "Usuário",
                    Sobrenome = nomes.Length > 1 ? string.Join(" ", nomes.Skip(1)) : "",
                    EmailConfirmed = true,
                    IsAtivo = true
                };

                var result = await _userManager.CreateAsync(usuario);

                if (!result.Succeeded)
                {
                    foreach (var erro in result.Errors)
                    {
                        Console.WriteLine(
                            $"ERRO IDENTITY: {erro.Code} - {erro.Description}");
                    }

                    return RedirectToPage("/Conta/Entrar");
                }
            }

            await _signInManager.SignInAsync(
                usuario,
                isPersistent: false);

            Console.WriteLine($"LOGIN EFETUADO: {email}");

            return Redirect("/");
        }

        public IActionResult OnPostGoogleLogin()
        {
            var redirectUrl = Url.Page(
                "/Conta/Entrar",
                pageHandler: "Callback");

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(
                    "Google",
                    redirectUrl);

            return Challenge(properties, "Google");
        }


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