using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using SiteAspas.Models;

public class MercadoPagoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MercadoPagoService> _logger;
    private readonly string _accessToken;
    private string ApiBaseUrl = "https://api.mercadopago.com/v1/payments";
    private const int PixExpirationMinutes = 30;

    public MercadoPagoService(
        HttpClient httpClient,
        IConfiguration config,
        ILogger<MercadoPagoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _accessToken = config["MercadoPago:AccessToken"];
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task<string> ObterBandeiraCartao(string numeroCartao)
    {
        var numeroLimpo = new string(numeroCartao.Where(char.IsDigit).ToArray());

        if (string.IsNullOrWhiteSpace(numeroLimpo) || numeroLimpo.Length < 4)
            return "desconhecida";

        var bin = numeroLimpo.Substring(0, 6);
        var primeirosDigitos = numeroLimpo.Substring(0, 2);
        var primeiroDigito = numeroLimpo[0];

        if (Regex.IsMatch(bin, @"^(5067|5090|6363|6362|4011[78]|4576|4571|4312|4389|4514|4573|5041[75])"))
            return "elo";

        if (Regex.IsMatch(bin, @"^(606282|3841[0-6])"))
            return "hipercard";

        if (Regex.IsMatch(numeroLimpo, @"^3[47]") && numeroLimpo.Length == 15)
            return "amex";

        if (Regex.IsMatch(primeirosDigitos, @"^(5[0-5])") ||
            Regex.IsMatch(numeroLimpo.Substring(0, 4), @"^(222[1-9]|22[3-9]|2[3-6]|27[0-1]|2720)"))
            return "master";

        if (primeiroDigito == '4')
            return "visa";

        return "desconhecida";
    }


    public async Task<PaymentResult> ProcessarPagamentoCartaoAsync(
    Pedido pedido,
    string numeroCartao,
    string validade,
    string cvv,
    string nomeTitular,
    int parcelas)
    {
        var cardNumber = new string(numeroCartao.Where(char.IsDigit).ToArray());
        var expiryParts = validade.Split('/');
        var expiryMonth = expiryParts[0];
        var expiryYear = $"20{expiryParts[1]}";
        var bandeira = await ObterBandeiraCartao(cardNumber);

        var paymentRequest = new
        {
            transaction_amount = pedido.Total,
            token = await ObterTokenCartao(cardNumber, expiryMonth, expiryYear, cvv, nomeTitular),
            description = $"Pedido #{pedido.Id} - SiteAspas",
            installments = parcelas,
            payment_method_id = bandeira,
            payer = new
            {
                email = "adrithome2@gmail.com",
                identification = new
                {
                    type = "CPF",
                    number = "12345678909"
                }
            }
        };

        var json = JsonSerializer.Serialize(paymentRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadopago.com/v1/payments")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        using var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;

        var result = new PaymentResult
        {
            Id = root.GetProperty("id").GetInt64(),
            Status = root.GetProperty("status").GetString(),
            StatusDetail = root.GetProperty("status_detail").GetString()
        };

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Erro ao processar pagamento com cartão: {Content}", content);
            result.Error = content;
        }

        return result;

    }

    private async Task<string> ObterTokenCartao(string cardNumber, string expiryMonth, string expiryYear, string cvv, string cardholderName)
    {
        var cardTokenRequest = new
        {
            card_number = cardNumber,
            expiration_month = expiryMonth,
            expiration_year = expiryYear,
            security_code = cvv,
            cardholder = new
            {
                name = cardholderName
            }
        };

        var json = JsonSerializer.Serialize(cardTokenRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadopago.com/v1/card_tokens")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao obter token do cartão: {content}");
        }

        using var jsonDoc = JsonDocument.Parse(content);
        return jsonDoc.RootElement.GetProperty("id").GetString();
    }

    public class PaymentResult
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public string Error { get; set; }
    }

    public async Task<PixPaymentResult> CriarPagamentoPixAsync(Pedido pedido)
    {
        try
        {
            var expirationDate = DateTime.UtcNow.AddMinutes(PixExpirationMinutes)
                .ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var body = new
            {
                transaction_amount = pedido.Total,
                description = $"Pedido #{pedido.Id} - {pedido.Usuario.Nome}",
                payment_method_id = "pix",
                date_of_expiration = expirationDate,
                payer = new
                {
                    email = pedido.Usuario.Email,
                    first_name = pedido.Usuario.Nome.Split(' ')[0],
                    last_name = pedido.Usuario.Nome.Split(' ').Last(),
                    identification = new
                    {
                        type = "CPF",
                        number = pedido.Usuario.CPF.Replace(".", "").Replace("-", "")
                    }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ApiBaseUrl)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json")
            };

            request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao criar pagamento PIX. Status: {StatusCode}. Response: {Content}",
                    response.StatusCode, content);
                throw new MercadoPagoException("Erro ao criar pagamento PIX", content);
            }

            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            return new PixPaymentResult
            {
                Id = root.GetProperty("id").ToString(),
                QrCode = root.GetProperty("point_of_interaction")
                     .GetProperty("transaction_data")
                     .GetProperty("qr_code")
                     .GetString() ?? string.Empty,
                QrCodeBase64 = root.GetProperty("point_of_interaction")
                           .GetProperty("transaction_data")
                           .GetProperty("qr_code_base64")
                           .GetString() ?? string.Empty,
                ExpirationDate = DateTime.Parse(root.GetProperty("date_of_expiration").GetString())
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar pagamento PIX");
            throw;
        }
    }

    public class PixPaymentResult
    {
        public string Id { get; set; }
        public string QrCode { get; set; }
        public string QrCodeBase64 { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public async Task<string> ObterStatusPagamentoAsync(string paymentId)
    {
        try
        {
            var url = $"https://api.mercadopago.com/v1/payments/{paymentId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro ao verificar pagamento {PaymentId}: {StatusCode} - {Error}",
                    paymentId, response.StatusCode, errorContent);
                throw new Exception($"Erro ao verificar pagamento: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);

            return jsonDoc.RootElement.GetProperty("status").GetString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar status do pagamento {PaymentId}", paymentId);
            throw;
        }
    }
}

public class MercadoPagoException : Exception
{
    public string ResponseContent { get; }

    public MercadoPagoException(string message, string responseContent)
        : base(message)
    {
        ResponseContent = responseContent;
    }
}