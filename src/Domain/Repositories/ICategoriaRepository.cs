using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria> ObterPorId(int id);
    }
}
