using SiteAspas.Models;

namespace SiteAspas.Services;

public class FreteService
{
    public decimal CalcularFrete(
        string cepDestino,
        IEnumerable<Produto> produtos)
    {
        decimal pesoTotal =
            produtos.Sum(p => p.Peso);

        decimal valorBase = 15m;

        decimal adicionalPeso =
            pesoTotal * 2m;

        return valorBase + adicionalPeso;
    }
}