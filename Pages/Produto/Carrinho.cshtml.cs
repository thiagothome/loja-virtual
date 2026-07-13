using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Data;
using SiteAspas.Models;

namespace SiteAspas.Pages;
[Authorize]
public class CarrinhoModel : PageModel
{
    private readonly SiteAspasContext _context;
    private readonly UserManager<Usuario> _userManager;

    public CarrinhoModel(
        SiteAspasContext context,
        UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<CarrinhoItem> Itens { get; set; } = new();

    public decimal Total =>
        Itens.Sum(i => (i.Preco ?? 0) * i.Quantidade);

    public async Task OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);

        if (usuario == null)
            return;

        Itens = await _context.CarrinhoItems
            .Where(x => x.ClienteId == usuario.Id)
            .OrderByDescending(x => x.DataAdicao)
            .ToListAsync();
    }

    [ValidateAntiForgeryToken]
public async Task<IActionResult> OnPostAumentarAsync(int id)
{
    var usuario = await _userManager.GetUserAsync(User);

    if (usuario == null)
        return Challenge();

    var item = await _context.CarrinhoItems
        .FirstOrDefaultAsync(x =>
            x.Id == id &&
            x.ClienteId == usuario.Id);

    if (item == null)
        return RedirectToPage();

    var produto = await _context.Produtos
        .FirstOrDefaultAsync(p => p.Id == item.ProdutoId);

    if (produto == null)
        return RedirectToPage();

    if (item.Quantidade >= produto.Estoque)
    {
        TempData["Erro"] =
            "Quantidade máxima disponível em estoque.";
        return RedirectToPage();
    }

    item.Quantidade++;

    await _context.SaveChangesAsync();

    return RedirectToPage();
}

[ValidateAntiForgeryToken]
public async Task<IActionResult> OnPostDiminuirAsync(int id)
{
    var usuario = await _userManager.GetUserAsync(User);

    if (usuario == null)
        return Challenge();

    var item = await _context.CarrinhoItems
        .FirstOrDefaultAsync(x =>
            x.Id == id &&
            x.ClienteId == usuario.Id);

    if (item == null)
        return RedirectToPage();

    item.Quantidade--;

    if (item.Quantidade <= 0)
    {
        _context.CarrinhoItems.Remove(item);
    }

    await _context.SaveChangesAsync();

    return RedirectToPage();
}

[ValidateAntiForgeryToken]
public async Task<IActionResult> OnPostRemoverAsync(int id)
{
    var usuario = await _userManager.GetUserAsync(User);

    if (usuario == null)
        return Challenge();

    var item = await _context.CarrinhoItems
        .FirstOrDefaultAsync(x =>
            x.Id == id &&
            x.ClienteId == usuario.Id);

    if (item == null)
        return RedirectToPage();

    _context.CarrinhoItems.Remove(item);

    await _context.SaveChangesAsync();

    return RedirectToPage();
}

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostFinalizarAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);

        if (usuario == null)
            return Challenge();

        if (!usuario.CadastroCompleto)
        {
            return RedirectToPage(
                "/Conta/CadastrarUsuarioCompleto",
                new
                {
                    returnUrl = Url.Page("/Pagamento/Index")
                });
        }

        return RedirectToPage("/Pagamento/Index");
    }
}