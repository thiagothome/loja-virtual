namespace SiteAspas.Models.ViewModel
{
    public class OpcaoFrete
    {
        public string Servico { get; set; }
        public decimal Valor { get; set; }
        public string PrazoEntrega { get; set; }
        public string CodigoServico { get; set; }
        public bool Disponivel => Valor > 0;
    }
}