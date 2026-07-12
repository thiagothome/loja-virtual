using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using SiteAspas.Models.Enums;

namespace SiteAspas.Models
{
    public class Usuario : IdentityUser<int>
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo 100 caracteres")]
        public string Sobrenome { get; set; }

        private string _email;
        private string _normalizedEmail;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(256, ErrorMessage = "O email deve ter no máximo 256 caracteres")]
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

        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "O CPF deve conter apenas números")]
        public string? CPF { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataNascimento { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? Telefone { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string? Senha { get; set; }

        public bool CadastroCompleto { get; set; } = false;

        [StringLength(256)]
        public string? EmailConfirmationToken { get; set; }

        public DateTime? TokenExpiration { get; set; }

        public bool IsAtivo { get; set; } = false;

        [Required]
        public TipoUsuario Tipo { get; set; } = TipoUsuario.Cliente;

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Endereco>? Enderecos { get; set; }
        public virtual ICollection<Pedido>? Pedidos { get; set; }
        
        [StringLength(50)]
        public string? CustomerId { get; set; }
    }
}