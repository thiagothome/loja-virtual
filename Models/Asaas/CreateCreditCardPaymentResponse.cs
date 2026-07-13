using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreateCreditCardPaymentResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("creditCardNumber")]
    public string? CreditCardNumber { get; set; }

    [JsonPropertyName("creditCardBrand")]
    public string? CreditCardBrand { get; set; }

    [JsonPropertyName("creditCardToken")]
    public string? CreditCardToken { get; set; }
}