using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreateBoletoPaymentResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("bankSlipUrl")]
    public string? BankSlipUrl { get; set; }

    [JsonPropertyName("nossoNumero")]
    public string? NossoNumero { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
