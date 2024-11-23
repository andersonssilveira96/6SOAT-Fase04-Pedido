using Application.DTOs.Pedido;
using Application.UseCase.Pedidos;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Producer;
using Domain.Repositories;
using Moq;

[Binding]
public class PedidoUseCaseSteps
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock = new();
    private readonly Mock<IProdutosRepository> _produtoRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessageBrokerProducer> _messageBrokerMock = new();
    private PedidoUseCase _useCase;
    private Exception _exception;
    private object _result;

    public PedidoUseCaseSteps()
    {
        _useCase = new PedidoUseCase(
            _pedidoRepositoryMock.Object,
            _produtoRepositoryMock.Object,
            _mapperMock.Object,
            _messageBrokerMock.Object
        );

        // Configurar o mapper para retornar um PedidoDto
        _mapperMock.Setup(m => m.Map<PedidoDto>(It.IsAny<Pedido>()))
                   .Returns((Pedido p) => new PedidoDto { Id = p.Id, Status = p.Status.ToString(), DataCriacao = p.DataCriacao });

        _mapperMock.Setup(m => m.Map<IEnumerable<PedidoDto>>(It.IsAny<IEnumerable<Pedido>>()))
           .Returns((IEnumerable<Pedido> pedidos) => pedidos.Select(p => new PedidoDto
           {
               Id = p.Id,
               Status = p.Status.ToString(),
               DataCriacao = p.DataCriacao
           }));
    }

    [Given(@"um pedido com ID (\d+) existe no repositório com status ""(.*)""")]
    public void DadoUmPedidoExisteNoRepositorioComStatus(long id, string status)
    {
        var pedido = new Pedido(id, Enum.Parse<StatusEnum>(status));

        _pedidoRepositoryMock.Setup(r => r.ObterPorId(id)).ReturnsAsync(pedido);
        _pedidoRepositoryMock.Setup(r => r.Atualizar(It.IsAny<Pedido>())).ReturnsAsync((Pedido p) => p);      
    }

    [When(@"o status do pedido é atualizado para ""(.*)""")]
    public async Task QuandoOStatusDoPedidoEAtualizadoPara(string status)
    {
        try
        {
            _result = await _useCase.AtualizarStatus(1, (int)Enum.Parse<StatusEnum>(status));
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"o status do pedido deve ser ""(.*)""")]
    public void EntaoOStatusDoPedidoDeveSer(string status)
    {
        var pedido = _result as PedidoDto;
        Assert.Equal(status, pedido.Status.ToString());
    }

    [Then(@"uma exceção deve ser lançada com a mensagem ""(.*)""")]
    public void EntaoUmaExcecaoDeveSerLancadaComAMensagem(string mensagem)
    {
        Assert.NotNull(_exception);
        Assert.Equal(mensagem, _exception.Message);
    }

    [Given(@"os produtos com IDs (\d+) e (\d+) existem no repositório")]
    public void DadoOsProdutosComIDsExistemNoRepositorio(long id1, long id2)
    {
        _produtoRepositoryMock.Setup(r => r.ObterPorId(id1)).ReturnsAsync(new Produto(id1, string.Empty, 0, null));
        _produtoRepositoryMock.Setup(r => r.ObterPorId(id2)).ReturnsAsync(new Produto(id2, string.Empty, 0, null));
    }

    [When(@"um pedido com esses produtos é cadastrado")]
    public async Task QuandoUmPedidoComEssesProdutosECadastrado()
    {
        var dto = new CadastrarPedidoDto
        {
            ClienteId = 1,
            Produtos = new List<PedidoProdutoDto>
            {
                new PedidoProdutoDto { ProdutoId = 1, Quantidade = 2, ValorUnitario = 1, Nome = "Teste", Observacao = string.Empty },
                new PedidoProdutoDto { ProdutoId = 2, Quantidade = 3, ValorUnitario = 2, Nome = "Teste 2", Observacao = string.Empty }
            },
            Viagem = false
        };

        try
        {
            _result = await _useCase.Inserir(dto);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"o pedido deve ser salvo com sucesso")]
    public void EntaoOPedidoDeveSerSalvoComSucesso()
    {
        _pedidoRepositoryMock.Verify(r => r.Inserir(It.IsAny<Pedido>()), Times.Once);
        Assert.Null(_exception);
    }

    [Then(@"uma mensagem deve ser enviada ao message broker")]
    public void EntaoUmaMensagemDeveSerEnviadaAoMessageBroker()
    {
        _messageBrokerMock.Verify(m => m.SendMessageAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Given(@"pedidos com diferentes status existem no repositório")]
    public void DadoPedidosComDiferentesStatusExistemNoRepositorio()
    {
        var pedidos = new List<Pedido>
        {
            new Pedido(),
            new Pedido()
        };

        pedidos.First().AtualizarStatus(StatusEnum.Cancelado);
        pedidos.Last().AtualizarStatus(StatusEnum.Recebido);

        _pedidoRepositoryMock.Setup(r => r.ListarPedidos()).ReturnsAsync(pedidos);
    }

    [When(@"a lista de pedidos é consultada")]
    public async Task QuandoAListaDePedidosEConsultada()
    {
        _result = await _useCase.Listar();
    }

    [Then(@"somente pedidos não finalizados devem ser retornados")]
    public void EntaoSomentePedidosNaoFinalizadosDevemSerRetornados()
    {
        var pedidos = _result as IEnumerable<PedidoDto>;
        Assert.All(pedidos, p => Assert.NotEqual(StatusEnum.Finalizado.ToString(), p.Status));
    }

    [Then(@"os pedidos devem ser ordenados por status e data de criação")]
    public void ThenOsPedidosDevemSerOrdenadosPorStatusEDataDeCriacao()
    {
        Assert.Null(_exception);

        Assert.NotNull(_result);

        var pedidos = _result as IEnumerable<PedidoDto>;
        Assert.NotNull(pedidos); 

        var pedidoList = pedidos.ToList();
        Assert.True(pedidoList.Count > 0); 

        var statusOrdenado = pedidoList
            .OrderBy(p => p.Status)
            .ThenByDescending(p => p.DataCriacao) 
            .ToList();

        Assert.Equal(pedidoList, statusOrdenado);
    }

    [When(@"o status de pagamento é consultado")]
    public async Task WhenOStatusDePagamentoEConsultado()
    {
        try
        {
            _result = await _useCase.ConsultarStatusPagamento(1); // Supondo que o ID do pedido é 1
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"o pagamento deve estar aprovado")]
    public void ThenOPagamentoDeveEstarAprovado()
    {
        Assert.Null(_exception); // Certifique-se de que nenhuma exceção foi lançada
        var statusPagamentoDto = _result as StatusPagamentoDto;
        Assert.NotNull(statusPagamentoDto);
        Assert.True(statusPagamentoDto.PagamentoAprovado);
    }
}