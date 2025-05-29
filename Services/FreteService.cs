using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.ViewModel;

namespace SiteAspas.Services
{
    public class FreteService : IFreteService
    {
        private readonly SiteAspasContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FreteService(SiteAspasContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<OpcaoFrete>> CalcularFrete(string cepOrigem, string cepDestino, List<CarrinhoItem> itens)
        {
            decimal pesoTotal = 0.2M;
            decimal alturaTotal = 0.15M;
            decimal larguraTotal = 0.08M;
            decimal comprimentoTotal = 0.08M;

            foreach (var item in itens)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                if (produto != null)
                {
                    pesoTotal += pesoTotal;
                    alturaTotal = alturaTotal;
                    larguraTotal = larguraTotal;
                    comprimentoTotal = comprimentoTotal;
                }
            }

            var apiUrl = _configuration.GetSection("FreteSettings")["ApiUrl"];
            var correiosApiKey = _configuration.GetSection("FreteSettings")["CorreiosApiKey"];

            // Construa a URL da API dos Correios, incluindo a chave de acesso como parâmetro
            // A forma exata de incluir a chave depende da API dos Correios.
            // Consulte a documentação da API dos Correios para saber como passar a chave (header, query parameter, etc.).
            var urlCompleta = $"{apiUrl}?nCdEmpresa={correiosApiKey}&sCepOrigem={cepOrigem}&sCepDestino={cepDestino}&nVlPeso={pesoTotal}&nCdFormato=1&nVlComprimento={comprimentoTotal}&nVlAltura={alturaTotal}&nVlLargura={larguraTotal}&sCdMaoPropria=N&nVlValorDeclarado=0&sCdAvisoRecebimento=N&nCdServico=04510&nVlDiametro=0"; // Exemplo genérico, adapte conforme a API


            try
            {
                var resposta = await _httpClient.GetFromJsonAsync<RespostaCalculoFrete>(urlCompleta);
                return resposta?.Opcoes;
            }
            catch (Exception ex)
            {
                // Logar o erro
                return null;
            }
        }
    }

    public class RespostaCalculoFrete
    {
        public List<OpcaoFrete> Opcoes { get; set; }
    }
}
