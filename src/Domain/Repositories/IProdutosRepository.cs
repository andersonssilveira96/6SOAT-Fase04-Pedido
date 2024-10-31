using Domain.Entities;

namespace Domain.Repositories
{
    public interface IProdutosRepository
    {
        Task<Produto> ObterPorId(long id);
        Task<List<Produto>> Listar();
        Task<Produto> Inserir(Produto produto);
        Task<Produto> Atualizar(Produto produto);
        Task Excluir(Produto produto);
        Task<List<Produto>> ListarPorCategoria(long idCategoria);
    }
}
