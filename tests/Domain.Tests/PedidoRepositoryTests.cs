using Domain.Entities;
using Domain.Enums;
using Infra.Data.Context;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

public class PedidoRepositoryTests
{
    private PedidoRepository _repository;
    private TechChallengeContext _context;

    public PedidoRepositoryTests()
    {
        // Configurando o banco InMemory
        var options = new DbContextOptionsBuilder<TechChallengeContext>()
            .UseInMemoryDatabase("TechChallengeTestDb")
            .Options;

        _context = new TechChallengeContext(options);

        // Preenchendo o banco com dados de teste                            
        if (!_context.Categoria.Any())
        {
            var categoria = new Categoria { Id = 2, Descricao = "Categoria Teste" };
            _context.Categoria.Add(categoria);
            _context.SaveChanges();
        }

        if (_context.Produto.Count() < 1)
        {
            var produto = new Produto(2, "Produto Teste", 10, _context.Categoria.First());
            _context.Produto.Add(produto);
            _context.SaveChanges();
        }

        if (!_context.Pedido.Any())
        {
            var produto = _context.Produto.First();
            var pedido = new Pedido(null, new List<PedidoProduto>
            {
                new PedidoProduto(produto.Id, 2, string.Empty,  produto)
            });

            _context.Pedido.Add(pedido);
            _context.SaveChanges();
        }       

        _repository = new PedidoRepository(_context);
    }

    [Fact]
    public async Task Inserir_DeveAdicionarPedidoComSucesso()
    {
        // Arrange
        var produto = _context.Produto.FirstOrDefault(x => x.Id == 2);
        var novoPedido = new Pedido(
            null,
            new List<PedidoProduto>
            {
                new PedidoProduto(produto.Id, 3, string.Empty, produto)
            }
        );
        

        // Act
        var pedidoInserido = await _repository.Inserir(novoPedido);

        // Assert
        Assert.NotNull(pedidoInserido);
        Assert.True(pedidoInserido.Id > 0);
        Assert.Equal(2, pedidoInserido.Produtos.First().ProdutoId);
        Assert.Equal(3, pedidoInserido.Produtos.First().Quantidade);
    }

    [Fact]
    public async Task Atualizar_DeveAtualizarPedidoComSucesso()
    {
        // Arrange
        var pedidoExistente = _context.Pedido.First();
        pedidoExistente.AtualizarStatus(StatusEnum.Cancelado);

        // Act
        var pedidoAtualizado = await _repository.Atualizar(pedidoExistente);

        // Assert
        Assert.NotNull(pedidoAtualizado);
        Assert.Equal(StatusEnum.Cancelado, pedidoAtualizado.Status);
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarTodosOsPedidos()
    {
        // Act
        var pedidos = await _repository.ListarPedidos();

        // Assert
        Assert.NotNull(pedidos);
        Assert.True(pedidos.Count > 0);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarPedido_QuandoIdExistir()
    {
        // Act
        var pedido = await _repository.ObterPorId(1);

        // Assert
        Assert.NotNull(pedido);
        Assert.Equal(1, pedido.Id);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNulo_QuandoIdNaoExistir()
    {
        // Act
        var pedido = await _repository.ObterPorId(99);

        // Assert
        Assert.Null(pedido);
    }
}
