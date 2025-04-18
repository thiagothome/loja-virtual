using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;
using SiteAspas;
using SiteAspas.Services;

var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<SiteAspasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages(); // Habilita Razor Pages

builder.Services.AddSingleton<ProdutoService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CarrinhoService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.MapGet("/produtos", async (SiteAspasContext db) =>
    await db.Produtos.ToListAsync());

app.MapPost("/produtos", async (Produto produto, SiteAspasContext db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produtos/{produto.Id}", produto);
});

app.MapGet("/Index", () => "Index");

app.UseStaticFiles();             // Habilita wwwroot
app.UseRouting();                 // Roteamento padrão
app.UseSession(); // <- AQUI é obrigatório
app.MapRazorPages();   

app.Run();
