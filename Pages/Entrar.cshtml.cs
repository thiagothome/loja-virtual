using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;
using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "O email � obrigat�rio")]
    [EmailAddress(ErrorMessage = "Email inv�lido")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "A senha � obrigat�ria")]
    [DataType(DataType.Password)]
    public string Senha { get; set; }

    [BindProperty]
    public bool LembrarMe { get; set; }

    public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    var usuario = await _userManager.FindByEmailAsync(Email);
    if (usuario == null)
    {
        ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
        return Page();
    }

    if (!await _userManager.IsEmailConfirmedAsync(usuario))
    {
        ModelState.AddModelError(string.Empty, "Você precisa confirmar seu e-mail antes de fazer login.");
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
        ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
        return Page();
    }

    return RedirectToPage("/Index");
}
}