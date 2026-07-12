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
             e.HasOne(p => p.Endereco)
                .WithMany()
                .HasForeignKey(p => p.EnderecoId)
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
            e.Property(c => c.Peso).HasPrecision(10, 2);
            e.Property(c => c.Altura).HasPrecision(10, 2);
            e.Property(c => c.Largura).HasPrecision(10, 2);
            e.Property(c => c.Comprimento).HasPrecision(10, 2);
        });

        var hasher = new PasswordHasher<Usuario>();
        var adminUser = new Usuario
        {
            Id = 1,
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            EmailConfirmed = true,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true,
            AccessFailedCount = 0,
            Nome = "Adriana",
            Sobrenome = "Thome",
            CPF = "",
            DataNascimento = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc),
            IsAtivo = true,
            Tipo = TipoUsuario.Administrador,
            DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EmailConfirmationToken = "SEED-TOKEN",
            TokenExpiration = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            SecurityStamp = "d1f6e1d0-b321-4bdf-bb0a-bf0000000000",
            ConcurrencyStamp = "aa87e1b9-e1c1-4a9b-91c9-ae0000000000"
        };

        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Aa200200@");

        modelBuilder.Entity<Usuario>().HasData(adminUser);

        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.NormalizedEmail).IsRequired();
            e.Property(u => u.NormalizedUserName).IsRequired();
            e.Property(u => u.ConcurrencyStamp).IsRequired();
            e.Property(u => u.SecurityStamp).IsRequired();
        });
    }
}