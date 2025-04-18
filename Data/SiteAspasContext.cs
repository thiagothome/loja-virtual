using Microsoft.EntityFrameworkCore;
using SiteAspas.Models;

namespace SiteAspas.Data;

public class SiteAspasContext : DbContext
{
    public SiteAspasContext(DbContextOptions<SiteAspasContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItems { get; set; }
    public DbSet<CarrinhoItem> CarrinhoItems { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Produto>(e => 
        {
            e.Property(p => p.Preco).HasPrecision(18, 2);
            e.Property(p => p.Nome).IsRequired();
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.Property(p => p.Total).HasPrecision(18, 2);
            e.Property(p => p.ClienteId).IsRequired();
        });

        modelBuilder.Entity<PedidoItem>(e => 
        {
            e.Property(p => p.PrecoUnitario).HasPrecision(18, 2);
            e.HasOne(pi => pi.Produto).WithMany().HasForeignKey(pi => pi.ProdutoId);
        });

        
        modelBuilder.Entity<CarrinhoItem>(e => 
        {
            e.HasNoKey(); 
            e.Property(c => c.Preco).HasPrecision(18, 2);
        });
    }
}