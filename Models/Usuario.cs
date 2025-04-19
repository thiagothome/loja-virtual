using SiteAspas.Models.Enums;

namespace SiteAspas.Models
{

public class Usuario
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string? CPF { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Telefone { get; set; }
    public string? Senha { get; set; }
    public bool IsAtivo { get; set; } = true;
    public TipoUsuario Tipo { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Endereco>? Enderecos { get; set; }
    public virtual ICollection<Pedido>? Pedidos { get; set; }
}
}