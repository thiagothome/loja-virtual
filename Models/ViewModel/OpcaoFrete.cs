namespace SiteAspas.Models.ViewModel
{
    public class OpcaoFrete
    {
        public string Servico { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public string PrazoEntrega { get; set; } = string.Empty;

        public string CodigoServico { get; set; } = string.Empty;

        public bool Disponivel => Valor > 0;
    }
}