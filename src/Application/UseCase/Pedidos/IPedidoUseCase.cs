using Application.DTOs;
using Application.DTOs.Pedido;
using Domain.Enums;

namespace Application.UseCase.Pedidos
{
    public interface IPedidoUseCase
    {
        Task<Result<object>> Inserir(CadastrarPedidoDto pedidoDto);
        Task<PedidoDto> EnviarPagamento(long id);
        Task<PedidoDto> AtualizarStatus(long id, int status);
        Task<IEnumerable<PedidoDto>> Listar();
        Task<StatusPagamentoDto> ConsultarStatusPagamento(long id);

    }
}
