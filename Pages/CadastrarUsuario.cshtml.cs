using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;

public class CadastrarUsuarioModel : PageModel
{
    private readonly SiteAspasContext _context;

    public CadastrarUsuarioModel(SiteAspasContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string NomeCompleto { get; set; }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Telefone { get; set; }

    [BindProperty]
    public string Senha { get; set; }

    [BindProperty]
    public string ConfirmarSenha { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Senha != ConfirmarSenha)
        {
            ModelState.AddModelError("ConfirmarSenha", "As senhas não coincidem.");
            return Page();
        }

        var hasher = new PasswordHasher<Usuario>();
        var novoUsuario = new Usuario
        {
            NomeCompleto = NomeCompleto,
            Email = Email,
            Telefone = Telefone
        };

        novoUsuario.Senha = hasher.HashPassword(novoUsuario, Senha);

        var emailExistente = await _context.Usuarios.AnyAsync(u => u.Email == Email);
        if (emailExistente)
        {
            ModelState.AddModelError("Email", "Este e-mail já está em uso.");
            return Page();
        }

        _context.Usuarios.Add(novoUsuario);
        await _context.SaveChangesAsync();

        // Redireciona para a página de login ou outra página de sucesso
        return RedirectToPage("/Entrar");
    }
}
