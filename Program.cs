using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;
using SiteAspas;
using SiteAspas.Services;
using SiteAspas.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<SiteAspasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages(); 

builder.Services.AddScoped<ProdutoService>(); 
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<SiteAspasContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.MapGet("/Index", () => "Index");

app.UseStaticFiles();             
app.UseRouting();                 
app.UseSession(); 
app.MapRazorPages();   

app.Run();
