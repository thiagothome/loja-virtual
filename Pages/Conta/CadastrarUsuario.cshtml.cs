using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using System.ComponentModel.DataAnnotations;
using SiteAspas.Services;


namespace SiteAspas.Pages.Conta
{
public class CadastrarUsuarioModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly IConfiguration _configuration;

    public CadastrarUsuarioModel(
        SiteAspasContext context,
        UserManager<Usuario> userManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [BindProperty]
    public Usuario Usuario { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O número de telefone é obrigatório")]
    [RegularExpression(@"^\(\d{2}\)\s\d{5}-\d{4}$",
        ErrorMessage = "Formato inválido. Use (XX) XXXXX-XXXX")]
    public string Telefone { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
    public string Senha { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Confirme a senha")]
    [DataType(DataType.Password)]
    [Compare("Senha", ErrorMessage = "As senhas não conferem")]
    public string ConfirmarSenha { get; set; }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuarioValidacao = await _userManager.FindByEmailAsync(Usuario.Email);
        if (usuarioValidacao?.Email == Usuario.Email)
        {
            ModelState.AddModelError("Usuario.Email", "Este e-mail já está cadastrado.");
            return Page();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var usuario = new Usuario
            {
                UserName = Usuario.Email,
                Email = Usuario.Email,
                NormalizedEmail = _userManager.NormalizeEmail(Usuario.Email),
                NormalizedUserName = _userManager.NormalizeName(Usuario.Email),
                Nome = Usuario.Nome,
                Sobrenome = Usuario.Sobrenome,
                Telefone = Telefone, // Salva com máscara
                Tipo = TipoUsuario.Cliente,
                IsAtivo = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(usuario, Senha);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("Senha",
                            "A senha não atende aos requisitos mínimos.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return Page();
            }

            await transaction.CommitAsync();

            return RedirectToPage("/Conta/Entrar");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(string.Empty, "Ocorreu um erro ao processar seu cadastro. Por favor, tente novamente.");
            return Page();
        }
    }
}
}