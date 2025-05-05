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
    public Usuario Usuario { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O número de telefone é obrigatório")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Telefone inválido")]
    [RegularExpression(@"^\(?\d{2}\)?[\s-]?\d{4,5}-?\d{4}$",
    ErrorMessage = "Formato inválido. Use (XX) XXXXX-XXXX")]
    public string Telefone { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
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
            ViewData["Senha"] = Usuario.Senha;
            ViewData["ConfirmarSenha"] = ConfirmarSenha;
            return Page();
        }

        var usuarioValidacao = await _userManager.FindByEmailAsync(Usuario.Email);
        if (usuarioValidacao?.Email == Usuario.Email)
        {
            ModelState.AddModelError("Usuario.Email", "Este e-mail já está cadastrado.");
            return Page();
        }

        var smtpSettings = _configuration.GetSection("SmtpSettings");
        if (!ValidateSmtpSettings(smtpSettings))
        {
            ModelState.AddModelError("Email", "Verifique o endereço de e-mail ou contate o suporte.");
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
                Nome = Usuario.Nome,
                Sobrenome = Usuario.Sobrenome,
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
                usuario.Nome,
                usuario.EmailConfirmationToken);

            await transaction.CommitAsync();

            return RedirectToPage("/Conta/EmailConfirmacao", new { email = usuario.Email });
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(string.Empty, "Ocorreu um erro ao processar seu cadastro. Por favor, tente novamente.");
            return Page();
        }
    }

    private bool ValidateSmtpSettings(IConfigurationSection smtpSettings)
    {
        var requiredSettings = new[] { "Host", "Port", "Username", "Password", "FromEmail", "FromName" };

        foreach (var setting in requiredSettings)
        {
            if (string.IsNullOrEmpty(smtpSettings[setting]))
            {
                return false;
            }
        }

        if (!int.TryParse(smtpSettings["Port"], out _))
        {
            return false;
        }

        return true;
    }

    private string GenerateToken()
    {
        return Guid.NewGuid().ToString("N") +
               Guid.NewGuid().ToString("N").Substring(0, 16);
    }
}