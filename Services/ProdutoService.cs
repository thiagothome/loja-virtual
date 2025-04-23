using SiteAspas.Models;
using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;

namespace SiteAspas.Services;

public class ProdutoService
{
    private readonly SiteAspasContext _context;

    public ProdutoService(SiteAspasContext context)
    {
        _context = context;
    }
    
    public async Task<List<Produto>> ObterDestaques()
    {
        return await _context.Produtos
            .OrderByDescending(p => p.Id) 
            .Take(10) 
            .ToListAsync();
    }
    
    public async Task<Produto?> ObterPorId(int id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task AdicionarProduto(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarProduto(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<List<Produto>> ObterProdutosPorUsuario(int usuarioId)
    {
        return await _context.Produtos
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync();
    }
}