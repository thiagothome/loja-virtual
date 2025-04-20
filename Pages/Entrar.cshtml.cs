using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;
using System.ComponentModel.DataAnnotations;

public class EntrarModel : PageModel
{
    private readonly SiteAspasContext _context;

    public EntrarModel(SiteAspasContext context)
    {
        _context = context;
    }

    [BindProperty]
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Senha { get; set; }

    [BindProperty]
    public bool LembrarMe { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == Email);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
            return Page();
        }

        var hasher = new PasswordHasher<Usuario>();
        var result = hasher.VerifyHashedPassword(usuario, usuario.Senha, Senha);

        if (result != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
            return Page();
        }

        // Cria a sessão do usuário
        HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
        HttpContext.Session.SetString("UsuarioNome", usuario.NomeCompleto);
        HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
        HttpContext.Session.SetString("UsuarioTipo", usuario.Tipo.ToString());

        return RedirectToPage("/Index");
    }
}