using Microsoft.EntityFrameworkCore;

using SiteAspas.Models;
using SiteAspas.Models.Enums;

namespace SiteAspas.Data;

public class SiteAspasContext : DbContext
{
    public SiteAspasContext(DbContextOptions<SiteAspasContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItems { get; set; }
    public DbSet<CarrinhoItem> CarrinhoItems { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().HasData(new Usuario
        {
            Id = 1,
            NomeCompleto = "Administrador do Sistema",
            Email = "admin@admin.com",
            Senha = "Admin123",
            IsAtivo = true,
            DataCadastro = DateTime.UtcNow,
            Tipo = TipoUsuario.Administrador
        });

        // Configura��o de Produto
        modelBuilder.Entity<Produto>(e =>
        {
            e.Property(p => p.Preco).HasPrecision(18, 2);
            e.Property(p => p.Nome).IsRequired();
            e.HasIndex(p => p.Nome);

            e.HasOne(p => p.Usuario)
            .WithMany() // ou .WithMany(u => u.Produtos) se quiser navegar do usuário para os produtos
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);
        });


        // Configura��o de Pedido
        modelBuilder.Entity<Pedido>(e =>
        {
            e.HasOne(p => p.Usuario)
             .WithMany(u => u.Pedidos)
             .HasForeignKey(p => p.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Configura��o de PedidoItem
        modelBuilder.Entity<PedidoItem>(e =>
        {
            e.Property(p => p.PrecoUnitario).HasPrecision(18, 2);
            e.HasOne(pi => pi.Produto)
             .WithMany()
             .HasForeignKey(pi => pi.ProdutoId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Configura��o de CarrinhoItem
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

       
    }
}