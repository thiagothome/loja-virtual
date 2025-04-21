using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteAspas.Models;
using SiteAspas.Models.Enums;

namespace SiteAspas.Data;

public class SiteAspasContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public SiteAspasContext(DbContextOptions<SiteAspasContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItems { get; set; }
    public DbSet<CarrinhoItem> CarrinhoItems { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UsuarioRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UsuarioClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UsuarioLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UsuarioTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        
        modelBuilder.Entity<Pedido>(e =>
        {
            e.Property(p => p.Total).HasPrecision(18, 2);
            e.HasOne(p => p.Usuario)
             .WithMany(u => u.Pedidos)
             .HasForeignKey(p => p.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PedidoItem>(e =>
        {
            e.Property(p => p.PrecoUnitario).HasPrecision(18, 2);
            e.HasOne(pi => pi.Produto)
             .WithMany()
             .HasForeignKey(pi => pi.ProdutoId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Produto>(e =>
        {
            e.Property(p => p.Preco).HasPrecision(18, 2);
            e.Property(p => p.Nome).IsRequired();
            e.HasIndex(p => p.Nome);
        });

        modelBuilder.Entity<CarrinhoItem>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Preco).HasPrecision(18, 2);
            e.HasOne(c => c.Produto)
             .WithMany()
             .HasForeignKey(c => c.ProdutoId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(c => c.ClienteId);
        });

        
        var hasher = new PasswordHasher<Usuario>();
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM", 
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin@123"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                NomeCompleto = "Administrador do Sistema",
                IsAtivo = true,
                Tipo = TipoUsuario.Administrador,
                DataCadastro = DateTime.UtcNow,
                EmailConfirmationToken = "SEED-TOKEN", 
                TokenExpiration = DateTime.UtcNow.AddYears(1) 
            });

        
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.NormalizedEmail).IsRequired();
            e.Property(u => u.NormalizedUserName).IsRequired();
            e.Property(u => u.ConcurrencyStamp).IsRequired();
            e.Property(u => u.SecurityStamp).IsRequired();
        });
    }
}