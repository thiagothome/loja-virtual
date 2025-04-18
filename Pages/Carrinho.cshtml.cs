using Microsoft.AspNetCore.Mvc.RazorPages;
using SiteAspas;

public class CarrinhoModel : PageModel
{
    private readonly CarrinhoService _carrinhoService;

    public CarrinhoModel(CarrinhoService carrinhoService)
    {
        _carrinhoService = carrinhoService;
    }

    public List<CarrinhoItem> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(i => i.Preco * i.Quantidade);

    public void OnGet()
    {
        Itens = _carrinhoService.ObterCarrinho();
    }
}
