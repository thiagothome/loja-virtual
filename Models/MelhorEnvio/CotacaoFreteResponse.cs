namespace SiteAspas.Models.MelhorEnvio;

public class CotacaoFreteResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public decimal Price { get; set; }

    public int DeliveryTime { get; set; }

    public string Company { get; set; } = "";
}