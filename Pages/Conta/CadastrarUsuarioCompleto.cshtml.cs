using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Data;
using SiteAspas.Models;

namespace SiteAspas.Pages
{
    [Authorize]
    public class CadastrarUsuarioCompletoModel : PageModel
    {
        private readonly SiteAspasContext _contexto;
        private readonly UserManager<Usuario> _userManager;

        public CadastrarUsuarioCompletoModel(SiteAspasContext contexto, UserManager<Usuario> userManager)
        {
            _contexto = contexto;
            _userManager = userManager;
        }


        public string Email { get; set; }

        [BindProperty]
        public string CPF { get; set; }

        [BindProperty]
        public DateTime? DataNascimento { get; set; }

        public string Telefone { get; set; }


        
        [BindProperty]
        public string CEP { get; set; }

        [BindProperty]
        public string Logradouro { get; set; }

        [BindProperty]
        public string Numero { get; set; }

        [BindProperty]
        public string Complemento { get; set; }

        [BindProperty]
        public string Bairro { get; set; }

        [BindProperty]
        public string Cidade { get; set; }

        [BindProperty]
        public string Estado { get; set; }

        [BindProperty]
        public bool Principal { get; set; } = true;

        [BindProperty]
        public Usuario Usuario { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            Usuario = user;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound();
    }

    
    user.CPF = CPF;
    user.DataNascimento = DataNascimento;
    user.CadastroCompleto = true; 

    
    var endereco = new Endereco
    {
        CEP = CEP,
        Logradouro = Logradouro,
        Numero = Numero,
        Complemento = Complemento,
        Bairro = Bairro,
        Cidade = Cidade,
        Estado = Estado,
        Principal = Principal,
        UsuarioId = user.Id
    };

    _contexto.Enderecos.Add(endereco);
    await _contexto.SaveChangesAsync();

    
    if (!string.IsNullOrEmpty(returnUrl))
    {
        return LocalRedirect(returnUrl);
    }
    
    return RedirectToPage("/Pagamento/Pagamento");
}
    }
}