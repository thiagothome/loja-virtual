using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreateBoletoPaymentRequest
{
    [JsonPropertyName("customer")]
    public string Customer { get; set; } = string.Empty;

    [JsonPropertyName("billingType")]
    public string BillingType { get; set; } = "BOLETO";

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime DueDate { get; set; }
}