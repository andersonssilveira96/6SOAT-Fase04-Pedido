using Domain.Repositories;
using Domain.Entities;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class ProdutosRepository : IProdutosRepository
    {
        private readonly TechChallengeContext _context;
        public ProdutosRepository(TechChallengeContext context)
        {
            _context = context;
        }

        public async Task<Produto> ObterPorId(long id) => await _context.Produto.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<List<Produto>> Listar() => await _context.Produto.Include(x => x.Categoria).ToListAsync();
        public async Task<Produto> Inserir(Produto produto)
        {
            if (produto is null)
            {
                throw new ArgumentNullException(nameof(produto));
            }

            _context.Produto.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<Produto> Atualizar(Produto produto)
        {
            _context.Produto.Entry(produto);          
            _context.Produto.Update(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task Excluir(Produto produto)
        {           
            if (produto is not null)
            {
                _context.Produto.Entry(produto).State = EntityState.Deleted;
                _context.Produto.Remove(produto);
            }

            await _context.SaveChangesAsync();           
        }

        public async Task<List<Produto>> ListarPorCategoria(long idCategoria)
        {
            var produtosCategoria =  _context.Produto.Include(x => x.Categoria).Where(x => x.Categoria.Id == idCategoria);
            return await produtosCategoria.ToListAsync();
        }
    }
}
