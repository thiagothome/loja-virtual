using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Services;
using SiteAspas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SiteAspasContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<SiteAspasContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Conta/Entrar";
    options.AccessDeniedPath = "/Erro/Erro";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    // Ajuste automático para desenvolvimento vs produção
    if (builder.Environment.IsDevelopment())
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Permite HTTP local
    }
    else
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
});

// Configurar Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".SiteAspas.Session";
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<AsaasService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Home/Index");
    options.Conventions.AllowAnonymousToPage("/Conta/Entrar");
    options.Conventions.AllowAnonymousToPage("/Conta/CadastrarUsuario");
    options.Conventions.AllowAnonymousToPage("/Erro/Erro");
    options.Conventions.AllowAnonymousToPage("/Conta/AtivarConta");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
    app.UseExceptionHandler("/Erro/Erro");
}

app.UseStatusCodePagesWithReExecute("/Erro/Erro", "?statusCode={0}");
// Desabilitar HTTPS em desenvolvimento
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();