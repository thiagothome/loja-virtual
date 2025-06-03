using Correios.Demo.Services.Models;
using SiteAspas.Data;
using SiteAspas.Models;
using SiteAspas.Models.ViewModel;
using System.Globalization;

namespace SiteAspas.Services
{
    public class FreteService : IFreteService
    {
        private readonly SiteAspasContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string _correiosUrl = "https://api.correios.com.br";

        private readonly string _usuario = "60765851000157";

        /// Obtenha o código aqui: https://cws.correios.com.br/dashboard/pesquisa
        private readonly string _codigoAcesso = "GNJmQ0RJjospOFbjJ9VGap7RHt8q6raHyUUD7Fti";

        /// Obtenha o cartão postagem e códigos aqui: https://sfe.correios.com.br/consultarContrato/consultarContrato.jsf#no-back-button
        private readonly string _cartaoPostagem = "0079308996";
        private readonly string _codigoServicoContratoPAC = "03085";
        private readonly string _codigoServicoContratoSedex = "03050";
        private readonly string _codigoServicoContratoSedex10 = "04740";
        private readonly string _codigoServicoContratoSedexHoje = "05703";
        private readonly string _codigoServicoContratoMiniEnvio = "03450";

        //TODO: armazenar no banco de dados
        private static string _contrato = "9912703149";
        private static int _dr;
        private static string _token = "eyJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE3NDg5NzkxMzMsImlzcyI6InRva2VuLXNlcnZpY2UiLCJleHAiOjE3NDkwNjU1MzMsImp0aSI6ImU5MDczMTNiLWQ5YmItNDIyYi05MjllLWM3YjE2ZGYxOGVmMSIsImFtYmllbnRlIjoiUFJPRFVDQU8iLCJwZmwiOiJQSiIsImlwIjoiMTM4Ljk3LjUuMTc4LCAxOTIuMTY4LjEuMTMxIiwiY2F0IjoiSW5kIiwiY29udHJhdG8iOnsibnVtZXJvIjoiOTkxMjcwMzE0OSIsImRyIjo2NCwiYXBpcyI6W3siYXBpIjo3OH0seyJhcGkiOjU2Nn0seyJhcGkiOjU4Nn0seyJhcGkiOjU4N31dfSwiaWQiOiI2MDc2NTg1MTAwMDE1NyIsImNucGoiOiI2MDc2NTg1MTAwMDE1NyJ9.mDSRVCLyrFj4jcf-tM8-vy949fFtgr5jtBTolh4YhDyuhgNCpm4Zups05Z-blHSSv_pfQjO506hazMHzjLH3nvULyz653hj-QfB2wWWp0Qo28YvXV7TIxhM3B8U9X2e7M88VQsKH1KAo0X6QmEfpNhOwDvz1RkIoyENnnvtZFMa1EJdAJ84-rkqBl_i1hDPqLJ5xGzOYSkSDIP6rG8m37HQb25iJBdhrPgG-Sd5qDssM4Ytl3zD5_LIjx5gPKD-q5Mr8Qcb524BWJlLGuKNCzNX5y6YVeu7JA7_9pfm-vSAhri57YfckejrBPEFcmjDVBZDPZFpVxpxzMF--bxMqwA";
        private static DateTime _expiracaotokenUTC;

        public FreteService(SiteAspasContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task<List<OpcaoFrete>> CalcularFrete(string cepOrigem, string cepDestino, List<CarrinhoItem> itens)
        {
            var volume = (decimal)0;

            decimal peso = 0;

            foreach (var item in itens)
            {
                var comprimentoItem = 16;
                var larguraItem =  11;
                var alturaItem = 2;

                
                var volumeItem = (alturaItem * larguraItem * comprimentoItem);

                
                volume += volumeItem;

                
                peso += (decimal)(200);
            }

            
            double potencia = 1.0 / 3.0;
            double raizCubica = Math.Pow(Convert.ToDouble(volume), potencia);

            var comprimento = Convert.ToDecimal(raizCubica);

            var altura = Convert.ToDecimal(raizCubica);

            var largura = Convert.ToDecimal(raizCubica);

            return await GetPrecoPrazo(cepOrigem, cepDestino, altura, largura, comprimento, peso);
        }

        private async Task<List<OpcaoFrete>> GetPrecoPrazo(string cepOrigem, string cepDestino, decimal altura, decimal largura, decimal comprimento, decimal peso)
        {
            var correiosService = new CorreiosService(_correiosUrl);

            
            UpdateCorreiosToken(correiosService);

            
            var codigoServicos = BuildCodigoServicosContrato();

            var precoRequest = new PrecoRequest();

            precoRequest.idLote = "1";
            precoRequest.parametrosProduto = new List<PrecoNacionalParam>();

            var prazoRequest = new PrazoRequest();

            prazoRequest.idLote = "1";
            prazoRequest.parametrosPrazo = new List<ParamPrazoNacional>();

            var token = _token;
            var contrato = _contrato;
            var dr = _dr;

            foreach (var codigoServico in codigoServicos)
            {
                var precoNacional = new PrecoNacionalParam
                {
                    cepOrigem = cepOrigem,
                    cepDestino = cepDestino,

                    nuContrato = contrato,
                    nuDR = dr,
                    nuRequisicao = "1",

                    tpObjeto = "2",
                    dtEvento = DateTime.Now.ToString("dd-MM-yyyy"),

                    altura = Math.Round(altura).ToString(),
                    largura = Math.Round(largura).ToString(),
                    diametro = "0",
                    comprimento = Math.Round(comprimento).ToString(),
                    psObjeto = Math.Round(peso).ToString(),
                    coProduto = codigoServico
                };

                var prazoNacional = new ParamPrazoNacional
                {
                    cepOrigem = cepOrigem,
                    cepDestino = cepDestino,
                    coProduto = codigoServico,
                    nuRequisicao = "1",
                    dtEvento = DateTime.Now.ToString("dd-MM-yyyy")
                };

                precoRequest.parametrosProduto.Add(precoNacional);
                prazoRequest.parametrosPrazo.Add(prazoNacional);
            }

            var precoResponse = await correiosService.GetPrecoAsync(precoRequest, token);

            var prazoResponse = correiosService.GetPrazo(prazoRequest, token);

            var opcoesEntrega = GerarOpcoesEntrega(precoResponse, prazoResponse);

            return opcoesEntrega;
        }

        private void UpdateCorreiosToken(CorreiosService correiosService)
        {
            var correiosToken = _token;
            var expiracaotokenUTC = _expiracaotokenUTC;

            if (string.IsNullOrEmpty(correiosToken) || CorreiosTokenExpired(expiracaotokenUTC))
            {
                var tokenResponse = correiosService.GetToken(_usuario, _codigoAcesso, _cartaoPostagem);

                _token = tokenResponse.token;
                _contrato = tokenResponse.cartaoPostagem.contrato;
                _dr = tokenResponse.cartaoPostagem.dr;
                _expiracaotokenUTC = tokenResponse.expiraEm;
            }
        }

        private bool CorreiosTokenExpired(DateTime expiracaotokenUTC)
        {
            bool expired = (expiracaotokenUTC <= DateTime.UtcNow.AddMinutes(-30));

            return expired;
        }

        private IList<string> BuildCodigoServicosContrato()
        {
            var codigos = new List<string>();

            if (!string.IsNullOrEmpty(_codigoServicoContratoPAC))
                codigos.Add(_codigoServicoContratoPAC);

            if (!string.IsNullOrEmpty(_codigoServicoContratoSedex))
                codigos.Add(_codigoServicoContratoSedex);

            if (!string.IsNullOrEmpty(_codigoServicoContratoSedex10))
                codigos.Add(_codigoServicoContratoSedex10);

            if (!string.IsNullOrEmpty(_codigoServicoContratoSedexHoje))
                codigos.Add(_codigoServicoContratoSedexHoje);

            if (!string.IsNullOrEmpty(_codigoServicoContratoMiniEnvio))
                codigos.Add(_codigoServicoContratoMiniEnvio);

            return codigos;
        }

        private List<OpcaoFrete> GerarOpcoesEntrega(IList<PrecoResponse> precos, IList<PrazoResponse> prazos)
        {
            var opcoesEntrega = new List<OpcaoFrete>();

            if (precos == null) return opcoesEntrega;

            foreach (var preco in precos)
            {
                if (preco == null || string.IsNullOrEmpty(preco.coProduto))
                    continue;

                
                decimal valor = 0;
                if (!string.IsNullOrEmpty(preco.pcFinal))
                {
                    decimal.TryParse(preco.pcFinal, NumberStyles.Any, CultureInfo.InvariantCulture, out valor);
                }

                if (valor > 0)
                {
                    var prazo = prazos?.FirstOrDefault(p => p?.coProduto == preco.coProduto);

                    string prazoText = "Não especificado";
                    if (prazo != null)
                    {
                        prazoText = prazo.prazoEntrega == 1
                            ? "1 dia útil"
                            : $"{prazo.prazoEntrega} dias úteis";
                    }

                    opcoesEntrega.Add(new OpcaoFrete
                    {
                        CodigoServico = preco.coProduto,
                        Servico = ObterNomeServico(preco.coProduto),
                        Valor = valor,
                        PrazoEntrega = prazoText
                    });
                }
            }

            return opcoesEntrega;
        }

        private string ObterNomeServico(string codigoServico)
        {
            string nomeServico = string.Empty;

            if (codigoServico == _codigoServicoContratoPAC)
            {
                nomeServico = "PAC";
                return nomeServico;
            }
            else if (codigoServico == _codigoServicoContratoSedex)
            {
                nomeServico = "Sedex";
                return nomeServico;
            }
            else if (codigoServico == _codigoServicoContratoSedex10)
            {
                nomeServico = "Sedex 10";
                return nomeServico;
            }
            else if (codigoServico == _codigoServicoContratoSedexHoje)
            {
                nomeServico = "Sedex Hoje";
                return nomeServico;
            }
            else if (codigoServico == _codigoServicoContratoMiniEnvio)
            {
                nomeServico = "Mini Envio";
                return nomeServico;
            }

            return nomeServico;
        }

        private string ObterPrazoEntrega(PrazoResponse prazo)
        {
            int diasEntrega = 0;

            if (prazo != null && string.IsNullOrEmpty(prazo.txErro))
            {
                diasEntrega = prazo.prazoEntrega;
            }

            if (diasEntrega == 0) return string.Empty;

            var diaUtil = diasEntrega == 1 ? "dia útil" : "dias úteis";

            var prazoEntrega = string.Format("até {0} {1}", diasEntrega, diaUtil);

            return prazoEntrega;
        }
    }

    public class RespostaCalculoFrete
    {
        public List<OpcaoFrete> Opcoes { get; set; }
    }
}
