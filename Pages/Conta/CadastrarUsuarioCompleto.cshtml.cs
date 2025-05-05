using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Data;
using SiteAspas.Models;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "O cpf é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O CPF deve conter apenas números")]
        [BindProperty]
        public string? CPF { get; set; }

        [Required(ErrorMessage = "a data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BindProperty]
        public DateTime? DataNascimento { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "CEP inválido")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "Formato de CEP inválido (XXXXX-XXX)")]
        public string CEP { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "O logradouro é obrigatório")]
        [StringLength(100, ErrorMessage = "O logradouro deve ter no máximo 100 caracteres")]
        public string Logradouro { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "O número é obrigatório")]
        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        [RegularExpression(@"^[0-9A-Za-z\s\-]+$", ErrorMessage = "Número inválido")]
        public string Numero { get; set; }

        [BindProperty]
        [StringLength(50, ErrorMessage = "O complemento deve ter no máximo 50 caracteres")]
        public string Complemento { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "O bairro é obrigatório")]
        [StringLength(50, ErrorMessage = "O bairro deve ter no máximo 50 caracteres")]
        public string Bairro { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "A cidade é obrigatória")]
        [StringLength(50, ErrorMessage = "A cidade deve ter no máximo 50 caracteres")]
        public string Cidade { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "O estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter 2 caracteres (UF)")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Informe a sigla do estado (ex: SP, RJ)")]
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

        [ValidateAntiForgeryToken]
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


            user.Nome = Usuario.Nome;
            user.Sobrenome = Usuario.Sobrenome;
            user.Email = Usuario.Email;
            user.CPF = CPF;
            user.DataNascimento = DataNascimento;
            user.Telefone = Usuario.Telefone;
            user.CadastroCompleto = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

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