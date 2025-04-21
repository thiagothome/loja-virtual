using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SiteAspas.Models.Enums;

namespace SiteAspas.Models
{
    public class Usuario : IdentityUser<int>
    {
        public string NomeCompleto { get; set; }

        private string _email;
        private string _normalizedEmail;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public override string Email
        {
            get => _email;
            set
            {
                _email = value;
                _normalizedEmail = value?.ToUpperInvariant();
            }
        }

        public override string NormalizedEmail
        {
            get => _normalizedEmail ?? Email?.ToUpperInvariant();
            set => _normalizedEmail = value;
        }
        public string? CPF { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }

        [NotMapped] 
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string? Senha { get; set; }
        [StringLength(256)]
        public string? EmailConfirmationToken { get; set; } 
        public DateTime? TokenExpiration { get; set; } 
        public bool IsAtivo { get; set; } = false;
        public TipoUsuario Tipo { get; set; } = TipoUsuario.Cliente;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Endereco>? Enderecos { get; set; }
        public virtual ICollection<Pedido>? Pedidos { get; set; }
    }
}