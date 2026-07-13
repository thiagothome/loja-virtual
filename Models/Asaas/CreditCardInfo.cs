using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreditCardInfo
{
    [JsonPropertyName("holderName")]
    public string HolderName { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("expiryMonth")]
    public string ExpiryMonth { get; set; } = string.Empty;

    [JsonPropertyName("expiryYear")]
    public string ExpiryYear { get; set; } = string.Empty;

    [JsonPropertyName("ccv")]
    public string Ccv { get; set; } = string.Empty;
}