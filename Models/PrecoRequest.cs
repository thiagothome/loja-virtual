namespace Correios.Demo.Services.Models
{
    public class PrecoRequest
    {
        public string idLote { get; set; }

        public IList<PrecoNacionalParam> parametrosProduto { get; set; }
    }

    public class PrecoNacionalParam
    {
        public string coProduto { get; set; }

        public string nuRequisicao { get; set; }

        public string nuContrato { get; set; }

        public int nuDR { get; set; }

        public string cepOrigem { get; set; }

        public string psObjeto { get; set; }

        public string nuUnidade { get; set; }

        public string tpObjeto { get; set; }

        public string comprimento { get; set; }

        public string largura { get; set; }

        public string altura { get; set; }

        public string diametro { get; set; }

        public string psCubico { get; set; }

        public string[] servicosAdicionais { get; set; }

        public string[] criterios { get; set; }

        public string vlDeclarado { get; set; }

        public string dtEvento { get; set; }

        public string coUnidadeOrigem { get; set; }

        public string dtArmazenagem { get; set; }

        public string cepDestino { get; set; }
    }
}
