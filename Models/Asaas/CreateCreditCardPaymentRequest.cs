using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreateCreditCardPaymentRequest
{
    [JsonPropertyName("customer")]
    public string Customer { get; set; } = string.Empty;

    [JsonPropertyName("billingType")]
    public string BillingType { get; set; } = "CREDIT_CARD";

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime DueDate { get; set; }

    [JsonPropertyName("creditCard")]
    public CreditCardInfo CreditCard { get; set; } = new();

    [JsonPropertyName("creditCardHolderInfo")]
    public CreditCardHolderInfo CreditCardHolderInfo { get; set; } = new();
}