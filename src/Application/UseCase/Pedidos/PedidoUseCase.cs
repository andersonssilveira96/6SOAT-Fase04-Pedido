using Application.DTOs;
using Application.DTOs.Pedido;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Producer;
using Domain.Repositories;

namespace Application.UseCase.Pedidos
{
    public class PedidoUseCase : IPedidoUseCase
    {
        private readonly IPedidoRepository _repository;
        private readonly IProdutosRepository _produtoRepository;
        private readonly IMapper _mapper;
        private readonly IMessageBrokerProducer _messageBrokerProducer;
        public PedidoUseCase(IPedidoRepository repository, IProdutosRepository produtoRepository, IMapper mapper, IMessageBrokerProducer messageBrokerProducer)
        {
            _repository = repository;
            _produtoRepository = produtoRepository;
            _mapper = mapper;
            _messageBrokerProducer = messageBrokerProducer;
        }        

        public async Task<PedidoDto> AtualizarStatus(long id, int status)
        {
            var pedido = await _repository.ObterPorId(id);

            if (pedido is null)
                throw new Exception($"PedidoId {id} inválido");

            if (!Enum.IsDefined(typeof(StatusEnum), status))
                throw new Exception($"Status {status} inválido");

            if (pedido.Status > (StatusEnum)status)
                throw new Exception($"Status não pode retroceder");

            pedido.AtualizarStatus((StatusEnum)status);

            return _mapper.Map<PedidoDto>(await _repository.Atualizar(pedido));
        }

        public async Task<Result<object>> Inserir(CadastrarPedidoDto pedidoDto)
        {           
            var pedidoProdutos = pedidoDto.Produtos.Select(x =>
            {
                var produto = _produtoRepository.ObterPorId(x.ProdutoId).GetAwaiter().GetResult();

                if (produto == null)
                    throw new Exception($"ProdutoId {x.ProdutoId} inválido");

                return new PedidoProduto(x.ProdutoId, x.Quantidade, x.Observacao, produto);
            }).ToList();

            var pedido = new Pedido(pedidoDto.ClienteId, pedidoProdutos, pedidoDto.Viagem);

            await _repository.Inserir(pedido);

            await _messageBrokerProducer.SendMessageAsync(pedido);            
   
            return new Result<object> { Mensagem = "Pedido cadastrado com sucesso" };
        }

        public async Task<IEnumerable<PedidoDto>> Listar()
        {
            var listaPedidos = await _repository.ListarPedidos();

            var filtrados = listaPedidos
                                .Where(x => x.Status != StatusEnum.Finalizado)
                                .OrderByDescending(x => x.Status)
                                .ThenBy(x => x.DataCriacao)
                                .ToList();

            return _mapper.Map<IEnumerable<PedidoDto>>(filtrados);
        }

        public async Task<StatusPagamentoDto> ConsultarStatusPagamento(long id)
        {
            var pedido = await _repository.ObterPorId(id);

            if (pedido is null)
                throw new Exception($"PedidoId {id} inválido");

            return new StatusPagamentoDto() { PagamentoAprovado = (pedido.Status != StatusEnum.PagamentoPendente && pedido.Status != StatusEnum.Cancelado) };
        }
    }
}
