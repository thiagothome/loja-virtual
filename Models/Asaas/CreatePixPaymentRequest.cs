using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreatePixPaymentRequest
{
    [JsonPropertyName("customer")]
    public string Customer { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("billingType")]
    public string BillingType { get; set; } = "PIX";

    [JsonPropertyName("dueDate")]
    public DateTime DueDate { get; set; }
}