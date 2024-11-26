using Domain.Entities;
using Infra.Data.Context;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Tests
{
    public class ProdutosRepositoryTests
    {
        private ProdutosRepository _repository;
        private TechChallengeContext _context;

        public ProdutosRepositoryTests()
        {
            // Configurando o banco InMemory
            var options = new DbContextOptionsBuilder<TechChallengeContext>()
                .UseInMemoryDatabase("TechChallengeTestDb")
                .Options;

            _context = new TechChallengeContext(options);

            // Preenchendo o banco com dados de teste                        
            if (!_context.Categoria.Any())
            {
                var categoria = new Categoria { Id = 3, Descricao = "Categoria Teste" };
                _context.Categoria.Add(categoria);
                _context.SaveChanges();
            }

            if (!_context.Produto.Any())
            {
                var produto = new Produto(1, "Produto Teste", 10, _context.Categoria.First());
                _context.Produto.Add(produto);
                _context.SaveChanges();
            }

            _repository = new ProdutosRepository(_context);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarProduto_QuandoProdutoExiste()
        {
            // Act
            var produto = await _repository.ObterPorId(1);

            // Assert
            Assert.NotNull(produto);
            Assert.Equal(1, produto.Id);
            Assert.Equal("Produto Teste", produto.Descricao);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNulo_QuandoProdutoNaoExiste()
        {
            // Act
            var produto = await _repository.ObterPorId(99);

            // Assert
            Assert.Null(produto);
        }

        [Fact]
        public async Task Listar_DeveRetornarTodosOsProdutos()
        {
            // Act
            var produtos = await _repository.Listar();

            // Assert
            Assert.NotNull(produtos);
            Assert.True(produtos.Count > 0);
        }

        [Fact]
        public async Task Inserir_DeveAdicionarProdutoComSucesso()
        {
            // Arrange
            var novoProduto = new Produto(4, "Produto Teste 2", 2, _context.Categoria.First());

            // Act
            var produtoInserido = await _repository.Inserir(novoProduto);

            // Assert
            Assert.NotNull(produtoInserido);
            Assert.True(produtoInserido.Id > 0);
            Assert.Equal("Produto Teste 2", produtoInserido.Descricao);
        }

        [Fact]
        public async Task Atualizar_DeveAtualizarProdutoComSucesso()
        {
            // Arrange
            var produtoExistente = _context.Produto.First();
            var categoria = _context.Categoria.Last();
            produtoExistente.AdicionarCategoria(categoria);

            // Act
            var produtoAtualizado = await _repository.Atualizar(produtoExistente);

            // Assert
            Assert.NotNull(produtoAtualizado);
            Assert.Equal(categoria, produtoAtualizado.Categoria);
        }

        [Fact]
        public async Task Excluir_DeveRemoverProdutoComSucesso()
        {
            // Arrange
            var produtoExistente = new Produto(88, "Produto Teste 2", 10, _context.Categoria.First());
            _context.Produto.Add(produtoExistente);
            _context.SaveChanges();

            // Act
            await _repository.Excluir(produtoExistente);
            var produtoRemovido = await _repository.ObterPorId(produtoExistente.Id);

            // Assert
            Assert.Null(produtoRemovido);
        }

        [Fact]
        public async Task ListarPorCategoria_DeveRetornarProdutosDaCategoria()
        {
            // Act
            var produtos = await _repository.ListarPorCategoria(1);

            // Assert
            Assert.NotNull(produtos);
            Assert.True(produtos.All(p => p.Categoria.Id == 1));
        }

        [Fact]
        public async Task ListarPorCategoria_DeveRetornarVazio_QuandoCategoriaNaoPossuiProdutos()
        {
            // Arrange
            var novaCategoria = new Categoria { Id = 55, Descricao = "Categoria Vazia" };
            _context.Categoria.Add(novaCategoria);
            _context.SaveChanges();

            // Act
            var produtos = await _repository.ListarPorCategoria(55);

            // Assert
            Assert.NotNull(produtos);
            Assert.Empty(produtos);
        }
    }

}
