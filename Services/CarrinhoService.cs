using SiteAspas;
using System.Text.Json;

public class CarrinhoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CarrinhoKey = "Carrinho";

    public CarrinhoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public List<CarrinhoItem> ObterCarrinho()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
        {
            throw new InvalidOperationException("Session não está disponível");
        }

        var carrinhoJson = session.GetString(CarrinhoKey);
        if (string.IsNullOrEmpty(carrinhoJson))
        {
            return new List<CarrinhoItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CarrinhoItem>>(carrinhoJson) 
                   ?? new List<CarrinhoItem>();
        }
        catch (JsonException)
        {
            return new List<CarrinhoItem>();
        }
    }

    public void AdicionarItem(CarrinhoItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var carrinho = ObterCarrinho();
        var itemExistente = carrinho.FirstOrDefault(i => i.ProdutoId == item.ProdutoId);
        
        if (itemExistente != null)
        {
            itemExistente.Quantidade += item.Quantidade;
        }
        else
        {
            carrinho.Add(item);
        }

        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
        {
            throw new InvalidOperationException("Session não está disponível");
        }

        session.SetString(CarrinhoKey, JsonSerializer.Serialize(carrinho));
    }
}