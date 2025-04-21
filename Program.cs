using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Services;
using SiteAspas.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SiteAspasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    options.LoginPath = "/Entrar";
    options.AccessDeniedPath = "/Erro"; 
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); 
    options.Conventions.AllowAnonymousToPage("/Index"); 
    options.Conventions.AllowAnonymousToPage("/Entrar");
    options.Conventions.AllowAnonymousToPage("/CadastrarUsuario");
    options.Conventions.AllowAnonymousToPage("/Erro");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404)
    {
        response.Redirect("/Erro");
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();