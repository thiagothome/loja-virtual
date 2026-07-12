using System.Text.Json.Serialization;

namespace SiteAspas.Models.Asaas;

public class PixResponse
{
    [JsonPropertyName("encodedImage")]
    public string? EncodedImage { get; set; }

    [JsonPropertyName("payload")]
    public string? Payload { get; set; }
}