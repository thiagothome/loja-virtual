using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreatePixPaymentResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}