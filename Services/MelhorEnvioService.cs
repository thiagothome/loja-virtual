using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SiteAspas.Models.MelhorEnvio;

namespace SiteAspas.Services;

public class MelhorEnvioService
{
    private readonly HttpClient _httpClient;
    private readonly MelhorEnvioSettings _settings;

    public MelhorEnvioService(
    HttpClient httpClient,
    IOptions<MelhorEnvioSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress =
            new Uri(_settings.BaseUrl);

        _httpClient.DefaultRequestHeaders.Clear();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _settings.AccessToken);

        _httpClient.DefaultRequestHeaders.Add(
            "Accept",
            "application/json");

        _httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "SiteAspas (thiago.thomers@gmail.com)");
    }

    public async Task<List<CotacaoFreteResponse>> CalcularFreteAsync(
    CotacaoFreteRequest request)
    {
        var body = new
        {
            from = new
            {
                postal_code = request.CepOrigem
            },

            to = new
            {
                postal_code = request.CepDestino
            },

            products = request.Produtos.Select(p => new
            {
                id = p.Id,
                width = p.Largura,
                height = p.Altura,
                length = p.Comprimento,
                weight = p.Peso,
                insurance_value = p.ValorDeclarado,
                quantity = p.Quantidade
            })
        };

        var json = JsonSerializer.Serialize(body);
        var response = await _httpClient.PostAsync(
            "/api/v2/me/shipment/calculate",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return new List<CotacaoFreteResponse>();

        using var doc = JsonDocument.Parse(content);

        var lista = new List<CotacaoFreteResponse>();

        foreach (var item in doc.RootElement.EnumerateArray())
        {
            decimal preco = 0;

            if (item.TryGetProperty("price", out var p))
            {
                if (p.ValueKind == JsonValueKind.Number)
                {
                    preco = p.GetDecimal();
                }
                else if (p.ValueKind == JsonValueKind.String)
                {
                    decimal.TryParse(
                        p.GetString(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out preco);
                }
            }
            int prazo = 0;

            if (item.TryGetProperty("delivery_time", out var t))
            {
                if (t.ValueKind == JsonValueKind.Number)
                {
                    prazo = t.GetInt32();
                }
                else if (t.ValueKind == JsonValueKind.String)
                {
                    int.TryParse(
                        t.GetString(),
                        out prazo);
                }
            }

            lista.Add(new CotacaoFreteResponse
            {
                Id = item.GetProperty("id").GetInt32(),

                Name = item.GetProperty("name").GetString() ?? "",

                Company =
                    item.GetProperty("company")
                        .GetProperty("name")
                        .GetString() ?? "",

                Price = preco,

                DeliveryTime = prazo
            });
        }

        return lista;
    }
}