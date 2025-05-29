using SiteAspas.Models;
using SiteAspas.Models.ViewModel; // Certifique-se de ter este using

namespace SiteAspas.Services
{
    public interface IFreteService
    {
        Task<List<OpcaoFrete>> CalcularFrete(string cepOrigem, string cepDestino, List<CarrinhoItem> itens);
    }
}