using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class CreateCustomerRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("mobilePhone")]
    public string? MobilePhone { get; set; }

    [JsonPropertyName("cpfCnpj")]
    public string? CpfCnpj { get; set; }
}