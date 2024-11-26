using Domain.Entities;
using Infra.Data.Context;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TechChallenge.Tests
{
    public class CategoriaRepositoryTests
    {
        private CategoriaRepository _repository;
        private TechChallengeContext _context;

        public CategoriaRepositoryTests()
        {
            // Configurando o banco InMemory
            var options = new DbContextOptionsBuilder<TechChallengeContext>()
                .UseInMemoryDatabase("TechChallengeTestDb")
                .Options;

            _context = new TechChallengeContext(options);

            // Preenchendo o banco com dados de teste
            if (!_context.Categoria.Any())
            {
                _context.Categoria.Add(new Categoria { Id = 2, Descricao = "Categoria Teste 2" });
                _context.SaveChanges();
            }

            _repository = new CategoriaRepository(_context);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarCategoria_QuandoCategoriaExiste()
        {
            // Arrange             
            _context.Categoria.Add(new Categoria { Id = 70, Descricao = "Categoria Teste 2" });
            _context.SaveChanges();            

            // Act
            var result = await _repository.ObterPorId(70);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(70, result.Id);
            Assert.Equal("Categoria Teste 2", result.Descricao);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNulo_QuandoCategoriaNaoExiste()
        {
            // Act
            var result = await _repository.ObterPorId(99);

            // Assert
            Assert.Null(result);
        }
    }

}
