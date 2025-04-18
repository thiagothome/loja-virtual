using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;
using SiteAspas;
using SiteAspas.Services;

var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<SiteAspasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages(); // Habilita Razor Pages

builder.Services.AddSingleton<ProdutoService>();


var app = builder.Build();

app.MapGet("/produtos", async (SiteAspasContext db) =>
    await db.Produtos.ToListAsync());

app.MapPost("/produtos", async (Produto produto, SiteAspasContext db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapPost("/carrinho", (ItemCarrinho item) =>
{
    CarrinhoFake.Itens.Add(item);
    return Results.Ok(CarrinhoFake.Itens);
});

app.MapGet("/carrinho", () =>
{
    return Results.Ok(CarrinhoFake.Itens);
});


app.MapGet("/Index", () => "Index");

app.UseStaticFiles();             // Habilita wwwroot
app.UseRouting();                 // Roteamento padrão
app.MapRazorPages();   

app.Run();
