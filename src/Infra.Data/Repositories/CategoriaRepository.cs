using Domain.Entities;
using Domain.Repositories;
using Infra.Data.Context;

namespace Infra.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly TechChallengeContext _context;
        public CategoriaRepository(TechChallengeContext context)
        {
            _context = context;
        }
        public async Task<Categoria> ObterPorId(int id) => await _context.Categoria.FindAsync(id);
    }
}
