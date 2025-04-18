namespace SiteAspas
{
public class ItemCarrinho
{
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public static class CarrinhoFake
{
    public static List<ItemCarrinho> Itens { get; set; } = new();
}
}