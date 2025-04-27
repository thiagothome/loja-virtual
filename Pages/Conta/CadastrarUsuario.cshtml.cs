using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.Enums;
using System.ComponentModel.DataAnnotations;
using SiteAspas.Services;

public class CadastrarUsuarioModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public CadastrarUsuarioModel(
        SiteAspasContext context,
        UserManager<Usuario> userManager,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
    }

    [BindProperty]
    [Required(ErrorMessage = "O nome completo é obrigatório")]
    [StringLength(100, ErrorMessage = "Máximo de 100 caracteres")]
    public string NomeCompleto { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [BindProperty]
    [RegularExpression(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$",
        ErrorMessage = "Telefone inválido. Use o formato (XX) XXXXX-XXXX")]
    public string Telefone { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mínimo 8 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Senha deve conter maiúsculas, minúsculas, números e símbolos")]
    public string Senha { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Confirme a senha")]
    [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmarSenha { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["Senha"] = Senha;
            ViewData["ConfirmarSenha"] = ConfirmarSenha;
            return Page();
        }

        var usuarioExistente = await _userManager.FindByEmailAsync(Email);
        if (usuarioExistente != null)
        {
            ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
            return Page();
        }

        var usuario = new Usuario
        {
            UserName = Email,
            Email = Email,
            NormalizedEmail = _userManager.NormalizeEmail(Email),
            NomeCompleto = NomeCompleto,
            Telefone = Telefone,
            Tipo = TipoUsuario.Cliente,
            IsAtivo = false,
            EmailConfirmationToken = GenerateToken(),
            TokenExpiration = DateTime.UtcNow.AddHours(24)
        };

        var result = await _userManager.CreateAsync(usuario, Senha);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await _emailService.EnviarEmailConfirmacaoAsync(
            usuario.Id.ToString(),
            usuario.Email,
            usuario.NomeCompleto,
            usuario.EmailConfirmationToken);

        return RedirectToPage("/Conta/EmailConfirmacao", new { email = usuario.Email });
    }

    private string GenerateToken()
    {
        return Guid.NewGuid().ToString("N") +
               Guid.NewGuid().ToString("N").Substring(0, 16);
    }
}