namespace SiteAspas.Models.MelhorEnvio;

public class ProdutoFreteRequest
{
    public string Id { get; set; } = "";

    public decimal Largura { get; set; }

    public decimal Altura { get; set; }

    public decimal Comprimento { get; set; }

    public decimal Peso { get; set; }

    public decimal ValorDeclarado { get; set; }

    public int Quantidade { get; set; }
}