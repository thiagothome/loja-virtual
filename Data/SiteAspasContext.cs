using Microsoft.EntityFrameworkCore;

namespace SiteAspas.Data;

public class SiteAspasContext : DbContext
{
    public SiteAspasContext(DbContextOptions<SiteAspasContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
}
