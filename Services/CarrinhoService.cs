using SiteAspas.Models;
using System.Text.Json;

public class CarrinhoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CarrinhoKey = "Carrinho";

    public CarrinhoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session
    {
        get
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                throw new InvalidOperationException("Sessão não está disponível. Verifique se: " +
                    "1. O middleware de sessão está configurado (app.UseSession()) " +
                    "2. Você está acessando de dentro de um contexto HTTP válido " +
                    "3. A requisição não está sendo feita em background");
            }
            return session;
        }
    }

    public List<CarrinhoItem> ObterCarrinho()
    {
        try
        {
            var carrinhoJson = Session.GetString(CarrinhoKey);
            return string.IsNullOrEmpty(carrinhoJson)
                ? new List<CarrinhoItem>()
                : JsonSerializer.Deserialize<List<CarrinhoItem>>(carrinhoJson) ?? new List<CarrinhoItem>();
        }
        catch (JsonException)
        {
            LimparCarrinho();
            return new List<CarrinhoItem>();
        }
    }


    public void AdicionarItem(CarrinhoItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

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

        SalvarCarrinho(carrinho);
    }

    public void SalvarCarrinho(List<CarrinhoItem> carrinho)
    {
        Session.SetString(CarrinhoKey, JsonSerializer.Serialize(carrinho));
    }

    public void LimparCarrinho()
    {
        Session.Remove(CarrinhoKey);
    }
}