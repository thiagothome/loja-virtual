using SiteAspas.Models;
using SiteAspas.Data;
using Microsoft.EntityFrameworkCore;

namespace SiteAspas.Services;

public class ProdutoService
{
    private readonly SiteAspasContext _context;

    // Injeta o contexto do banco de dados
    public ProdutoService(SiteAspasContext context)
    {
        _context = context;
    }

    // Obtém produtos do banco de dados (não mais da lista fixa)
    public async Task<List<Produto>> ObterDestaques()
    {
        return await _context.Produtos
            .OrderByDescending(p => p.Id) // Ordena por mais recentes
            .Take(10) // Pega os 10 mais recentes
            .ToListAsync();
    }

    // Obtém um produto por ID do banco de dados
    public async Task<Produto?> ObterPorId(int id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    // Novo método para adicionar um produto
    public async Task AdicionarProduto(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
    }

    // Novo método para atualizar um produto
    public async Task AtualizarProduto(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    // Novo método para remover um produto
    public async Task RemoverProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }

    // Novo método para obter produtos de um usuário específico
    public async Task<List<Produto>> ObterProdutosPorUsuario(int usuarioId)
    {
        return await _context.Produtos
            .Where(p => p.UsuarioId == usuarioId)
            .ToListAsync();
    }
}