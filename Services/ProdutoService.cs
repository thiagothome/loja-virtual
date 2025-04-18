namespace SiteAspas.Services;

public class ProdutoService
{
    public List<Produto> ObterDestaques()
    {
        return new List<Produto>
        {
            new Produto { 
                Id = 1, 
                Nome = "Blusa Estampada", 
                ImagemUrl = "/img/blusa.jpg", 
                Preco = 39.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."

            },
            new Produto { 
                Id = 2, 
                Nome = "Vestido Floral", 
                ImagemUrl = "/img/vestido.jpg", 
                Preco = 49.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 3, 
                Nome = "Colar Artesanal", 
                ImagemUrl = "/img/colar.jpg", 
                Preco = 19.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 4, 
                Nome = "Colar Artesanal", 
                ImagemUrl = "/img/colar.jpg", 
                Preco = 19.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 5, 
                Nome = "Colar Artesanal", 
                ImagemUrl = "/img/colar.jpg", 
                Preco = 19.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 6, 
                Nome = "Blusa Estampada", 
                ImagemUrl = "/img/blusa.jpg", 
                Preco = 39.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 7, 
                Nome = "Vestido Floral", 
                ImagemUrl = "/img/vestido.jpg", 
                Preco = 49.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 8, 
                Nome = "Colar Artesanal", 
                ImagemUrl = "/img/colar.jpg", 
                Preco = 19.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 9, 
                Nome = "Colar Artesanal", 
                ImagemUrl = "/img/colar.jpg", 
                Preco = 19.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            },
            new Produto { 
                Id = 10, 
                Nome = "Colar Artesanal", 
                ImagemUrl = "/img/colar.jpg", 
                Preco = 19.90m,
                Descricao = "Blusa com estampa floral, tecido leve e confortável."
            }
        };
    }

    public Produto? ObterPorId(int id)
    {
        return ObterDestaques().FirstOrDefault(p => p.Id == id);
    }
}
