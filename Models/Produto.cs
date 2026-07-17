using System.ComponentModel.DataAnnotations;

namespace SiteAspas.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome � obrigat�rio.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no m�ximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public string ImagemUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "O pre�o � obrigat�rio.")]
        [Range(0.01, 999999.99, ErrorMessage = "O pre�o deve ser maior que zero.")]
        public decimal? Preco { get; set; }

        [Required(ErrorMessage = "O estoque � obrigat�rio.")]
        [Range(0, int.MaxValue, ErrorMessage = "O estoque n�o pode ser negativo.")]
        public int? Estoque { get; set; }

        [Required(ErrorMessage = "O peso é obrigatório.")]
        [Range(0.001, 9999)]
        public decimal Peso { get; set; }

        [Required(ErrorMessage = "A altura é obrigatória.")]
        [Range(1, 999)]
        public decimal Altura { get; set; }

        [Required(ErrorMessage = "A largura é obrigatória.")]
        [Range(1, 999)]
        public decimal Largura { get; set; }

        [Required(ErrorMessage = "O comprimento é obrigatório.")]
        [Range(1, 999)]
        public decimal Comprimento { get; set; }

        [Required(ErrorMessage = "A descri��o � obrigat�ria.")]
        [StringLength(1000, ErrorMessage = "A descri��o deve ter no m�ximo 1000 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public int? UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
