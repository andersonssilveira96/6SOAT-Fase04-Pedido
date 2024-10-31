using Application.DTOs.Produtos;

namespace Application.UseCase.Produtos
{
    public interface IProdutosUseCase
    {
        Task<ProdutoDto> Obter(long id);
        Task<List<ProdutoDto>> Listar();
        Task<List<ProdutoDto>> ListarPorCategoria(long idCategoria);
        Task<ProdutoDto> Cadastrar(CadastrarProdutoDto produto);
        Task<ProdutoDto> Atualizar(AtualizarProdutoDto produto, long id);
        Task Excluir(long id);
    }
}