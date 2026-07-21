namespace SiteAspas.Models.MelhorEnvio;

public class CotacaoFreteRequest
{
    public string CepOrigem { get; set; } = "";

    public string CepDestino { get; set; } = "";

    public List<ProdutoFreteRequest> Produtos { get; set; } = new();
}