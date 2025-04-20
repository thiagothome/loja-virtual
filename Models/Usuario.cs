using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SiteAspas.Models.Enums;

namespace SiteAspas.Models
{
    public class Usuario : IdentityUser<int>
    {
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        public string? CPF { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }

        [NotMapped] // Isso evita que seja mapeada para o banco
        [Required(ErrorMessage = "A senha é obrigatória")]
        public string? Senha { get; set; }
        [StringLength(256)]
        public string? EmailConfirmationToken { get; set; } // Token como string        public DateTime? TokenExpiration { get; set; }
        public DateTime? TokenExpiration { get; set; } // Nullable para casos onde o token ainda não foi gerado
        public bool IsAtivo { get; set; } = false;
        public TipoUsuario Tipo { get; set; } = TipoUsuario.Cliente;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Endereco>? Enderecos { get; set; }
        public virtual ICollection<Pedido>? Pedidos { get; set; }
    }
}