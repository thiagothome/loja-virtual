using Correios.Demo.Services.Models;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace SiteAspas.Services
{
    public class CorreiosService
    {
        private readonly string _correiosUrl;

        public CorreiosService(string correiosUrl)
        {
            _correiosUrl = correiosUrl;
        }
        
        public TokenResponse GetToken(string usuario, string codigoAcesso, string cartaoPostagem)
        {
            TokenResponse tokenResponse = null;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), _correiosUrl + "/token/v1/autentica/cartaopostagem"))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");

                        var codigo = Convert.ToBase64String(Encoding.UTF8.GetBytes(usuario + ":" + codigoAcesso));

                        request.Headers.TryAddWithoutValidation("Authorization", "Basic " + codigo);

                        var tokenRequest = new TokenRequest()
                        {
                            numero = cartaoPostagem
                        };

                        var body = JsonConvert.SerializeObject(tokenRequest);

                        request.Content = new StringContent(body);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = httpClient.SendAsync(request).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            tokenResponse = JsonConvert .DeserializeObject<TokenResponse>(responseBody);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return tokenResponse;
        }

        public async Task<IList<PrecoResponse>> GetPrecoAsync(PrecoRequest precoRequest, string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _correiosUrl + "/preco/v1/nacional"))
                    {
                        request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + token);

                        var body = JsonConvert.SerializeObject(precoRequest);
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                        var response = await client.SendAsync(request);

                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode ||
                            response.StatusCode == HttpStatusCode.PartialContent)
                        {
                            return JsonConvert.DeserializeObject<List<PrecoResponse>>(responseBody)
                                ?? new List<PrecoResponse>();
                        }

                        
                        if (response.StatusCode == HttpStatusCode.BadRequest ||
                            response.StatusCode == HttpStatusCode.InternalServerError ||
                            response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                try
                                {
                                    
                                    if (responseBody.Contains("path"))
                                    {
                                        var error = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                                        
                                        Console.WriteLine($"Erro na API Correios: {error}");
                                    }
                                    else
                                    {
                                        
                                        return JsonConvert.DeserializeObject<List<PrecoResponse>>(responseBody)
                                            ?? new List<PrecoResponse>();
                                    }
                                }
                                catch (JsonException jsonEx)
                                {
                                    
                                    Console.WriteLine($"Erro ao desserializar resposta: {jsonEx.Message}");
                                }
                            }
                        }

                        
                        Console.WriteLine($"Status code não tratado: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Exceção ao chamar API Correios: {ex}");
            }

            return new List<PrecoResponse>(); 
        }

        public IList<PrazoResponse> GetPrazo(PrazoRequest precoRequest, string token)
        {
            List<PrazoResponse> precoResponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _correiosUrl + "/prazo/v1/nacional"))
                    {
                        request.Headers.Add("Accept", "application/json");
                        request.Headers.Add("Authorization", "Bearer " + token);

                        var body = JsonConvert.SerializeObject(precoRequest);

                        var content = new StringContent(body);
                        request.Content = content;
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = client.SendAsync(request).Result;

                        if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                            response.StatusCode == System.Net.HttpStatusCode.PartialContent)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            precoResponse = JsonConvert.DeserializeObject<List<PrazoResponse>>(responseBody);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                            response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                if (responseBody.Contains("path"))
                                {
                                    var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
                                }
                                else
                                {
                                    precoResponse = JsonConvert.DeserializeObject<List<PrazoResponse>>(responseBody);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return precoResponse;
        }
    }
}
