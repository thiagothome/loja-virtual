using System.Linq;
using System.Text;
using System.Text.Json;
using SiteAspas.Models.Asaas;

namespace SiteAspas.Services;

public class AsaasService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AsaasService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        var baseUrl = _configuration["Asaas:BaseUrl"]!;
        if (!baseUrl.EndsWith("/"))
            baseUrl += "/";

        _httpClient.BaseAddress = new Uri(baseUrl);

        _httpClient.DefaultRequestHeaders.Clear();

        _httpClient.DefaultRequestHeaders.Add(
            "access_token",
            _configuration["Asaas:ApiKey"]);

        // Adicionar User-Agent obrigatório
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "SiteAspas/1.0");

        Console.WriteLine(
            $"ASAAS URL: {_httpClient.BaseAddress}");
        Console.WriteLine(
            $"ASAAS TOKEN: {(_configuration["Asaas:ApiKey"]?.Length ?? 0)} caracteres");
    }

    public async Task<string?> CriarClienteAsync(
    string nome,
    string email,
    string telefone,
    string cpf)
    {
        // Limpar telefone: remover tudo que não for número
        var telefoneLimpo = string.IsNullOrEmpty(telefone)
            ? ""
            : new string(telefone.Where(char.IsDigit).ToArray());

        // Se o telefone não começar com 55, adicionar
        if (!string.IsNullOrEmpty(telefoneLimpo) && !telefoneLimpo.StartsWith("55"))
        {
            telefoneLimpo = "55" + telefoneLimpo;
        }

        // Validar: se não tiver 13 dígitos (55 + DDD + 9 dígitos), não enviar
        if (telefoneLimpo.Length != 13)
        {
            Console.WriteLine($"Telefone inválido ({telefoneLimpo.Length} dígitos): {telefoneLimpo}");
            telefoneLimpo = "";
        }

        var cpfLimpo = string.IsNullOrEmpty(cpf)
            ? ""
            : new string(cpf.Where(char.IsDigit).ToArray());

        var request = new CreateCustomerRequest
        {
            Name = nome,
            Email = email,
            MobilePhone = telefoneLimpo,
            CpfCnpj = cpfLimpo
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"Criando cliente: {json}");

        var response =
            await _httpClient.PostAsync(
                "customers",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"ASAAS ERRO [CriarCliente]: {response.StatusCode}");
            Console.WriteLine(erro);

            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Cliente criado: {content}");

        var customer =
            JsonSerializer.Deserialize<CreateCustomerResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return customer?.Id;
    }

    public async Task<string?> CriarCobrancaPixAsync(
        string customerId,
        decimal valor)
    {
        var request = new CreatePixPaymentRequest
        {
            Customer = customerId,
            Value = valor,
            DueDate = DateTime.Today.AddDays(1),
            BillingType = "PIX"
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"Criando cobrança: {json}");

        var response =
            await _httpClient.PostAsync(
"payments",
new StringContent(
    json,
    Encoding.UTF8,
    "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"ASAAS ERRO [CriarCobranca]: {response.StatusCode}");
            Console.WriteLine(erro);

            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Cobrança criada: {content}");

        var payment =
            JsonSerializer.Deserialize<CreatePixPaymentResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return payment?.Id;
    }

    public async Task<PixResponse?> ObterQrCodePixAsync(
        string paymentId)
    {
        Console.WriteLine($"Buscando QR Code para pagamento: {paymentId}");

        var response =
            await _httpClient.GetAsync(
                $"payments/{paymentId}/pixQrCode");

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"ASAAS ERRO [ObterQrCode]: {response.StatusCode}");
            Console.WriteLine(erro);

            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"QR Code obtido: {content}");

        return JsonSerializer.Deserialize<PixResponse>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<string?> CriarCobrancaBoletoAsync(
    string customerId,
    decimal valor)
    {
        var request = new CreateBoletoPaymentRequest
        {
            Customer = customerId,
            Value = valor,
            DueDate = DateTime.Today.AddDays(3), // Vencimento em 3 dias
            BillingType = "BOLETO"
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"Criando boleto: {json}");

        var response =
            await _httpClient.PostAsync(
                "payments",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"ASAAS ERRO [CriarBoleto]: {response.StatusCode}");
            Console.WriteLine(erro);

            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Boleto criado: {content}");

        var payment =
            JsonSerializer.Deserialize<CreateBoletoPaymentResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return payment?.Id;
    }

    public async Task<CreateBoletoPaymentResponse?> ObterBoletoAsync(
        string paymentId)
    {
        Console.WriteLine($"Buscando boleto para pagamento: {paymentId}");

        var response =
            await _httpClient.GetAsync(
                $"payments/{paymentId}");

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"ASAAS ERRO [ObterBoleto]: {response.StatusCode}");
            Console.WriteLine(erro);

            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Boleto obtido: {content}");

        return JsonSerializer.Deserialize<CreateBoletoPaymentResponse>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<string?> ObterLinhaDigitavelAsync(string paymentId)
    {
        Console.WriteLine($"Buscando linha digitável para: {paymentId}");

        var response =
            await _httpClient.GetAsync(
                $"payments/{paymentId}/identificationField");

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();
            Console.WriteLine(
                $"ASAAS ERRO [LinhaDigitavel]: {response.StatusCode}");
            Console.WriteLine(erro);
            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Linha digitável: {content}");

        var result = JsonSerializer.Deserialize<JsonElement>(content);
        return result.GetProperty("identificationField").GetString();
    }

    public async Task<CreateCreditCardPaymentResponse?> CriarCobrancaCartaoAsync(
        string customerId,
        decimal valor,
        string holderName,
        string number,
        string expiryMonth,
        string expiryYear,
        string ccv,
        string holderEmail,
        string holderCpfCnpj,
        string postalCode,
        string addressNumber,
        string phone)
    {
        var request = new CreateCreditCardPaymentRequest
        {
            Customer = customerId,
            Value = valor,
            DueDate = DateTime.UtcNow.AddDays(1),
            BillingType = "CREDIT_CARD",
            CreditCard = new CreditCardInfo
            {
                HolderName = holderName,
                Number = number,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                Ccv = ccv
            },
            CreditCardHolderInfo = new CreditCardHolderInfo
            {
                Name = holderName,
                Email = holderEmail,
                CpfCnpj = holderCpfCnpj,
                PostalCode = postalCode,
                AddressNumber = addressNumber,
                Phone = phone
            }
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"Criando cobrança cartão: {json}");

        var response =
            await _httpClient.PostAsync(
                "payments",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var erro =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine(
                $"ASAAS ERRO [CriarCartao]: {response.StatusCode}");
            Console.WriteLine(erro);

            return null;
        }

        var content =
            await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Cobrança cartão criada: {content}");

        return JsonSerializer.Deserialize<CreateCreditCardPaymentResponse>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<bool> SimularPagamentoPixAsync(string paymentId, decimal valor, DateTime dataPagamento)
    {
        var request = new
        {
            value = valor,
            paymentDate = dataPagamento.ToString("yyyy-MM-dd")
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"Simulando pagamento: {json}");

        var response = await _httpClient.PostAsync(
            $"payments/{paymentId}/receiveInCash",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"ERRO ao simular pagamento: {erro}");
            return false;
        }

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Pagamento simulado com sucesso! Resposta: {content}");
        return true;
    }
}