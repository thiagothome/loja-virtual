namespace SiteAspas.Models.MelhorEnvio;

public class CalculoFreteRequest
{
    public FromRequest From { get; set; } = new();

    public ToRequest To { get; set; } = new();

    public List<ProdutoFreteRequest> Products { get; set; } = new();
}

public class FromRequest
{
    public string PostalCode { get; set; } = "";
}

public class ToRequest
{
    public string PostalCode { get; set; } = "";
}