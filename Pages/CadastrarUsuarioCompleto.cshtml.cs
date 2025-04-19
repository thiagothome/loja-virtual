using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas.Models;

namespace SiteAspas.Pages
{
    public class CadastrarUsuarioCompleto : PageModel
    {
        [BindProperty]
        public string NomeCompleto { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string CPF { get; set; }

        [BindProperty]
        public DateTime? DataNascimento { get; set; }

        [BindProperty]
        public string Telefone { get; set; }

        [BindProperty]
        public string Senha { get; set; }

        [BindProperty]
        public string ConfirmarSenha { get; set; }

        // Dados de Endereço
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

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lógica para salvar no banco de dados
            // Exemplo:
            var usuario = new Usuario
            {
                NomeCompleto = NomeCompleto,
                Email = Email,
                CPF = CPF,
                DataNascimento = DataNascimento,
                Telefone = Telefone,
                Senha = Senha,
                Enderecos = new List<Endereco>
                {
                    new Endereco
                    {
                        CEP = CEP,
                        Logradouro = Logradouro,
                        Numero = Numero,
                        Complemento = Complemento,
                        Bairro = Bairro,
                        Cidade = Cidade,
                        Estado = Estado,
                        Principal = Principal
                    }
                }
            };

            // Aqui você salvaria no banco usando Entity Framework ou outro método

            return RedirectToPage("/Sucesso"); // Redireciona após cadastro
        }
    }
}