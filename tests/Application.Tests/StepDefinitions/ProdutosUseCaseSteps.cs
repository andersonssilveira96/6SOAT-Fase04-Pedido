using Application.DTOs.Produtos;
using Application.UseCase.Produtos;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Moq;

[Binding]
public class ProdutosUseCaseSteps
{
    private readonly IProdutosUseCase _produtosUseCase;
    private readonly Mock<IProdutosRepository> _produtosRepositoryMock;
    private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
    private readonly IMapper _mapper;
    private ProdutoDto _resultadoCadastro;
    private ProdutoDto _produtoConsultado;
    private List<ProdutoDto> _resultadoLista;
    private ProdutoDto _produtoAtualizado;
    private Exception _excecao;

    public ProdutosUseCaseSteps()
    {
        _produtosRepositoryMock = new Mock<IProdutosRepository>();
        _categoriaRepositoryMock = new Mock<ICategoriaRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Categoria, CategoriaDto>();
            cfg.CreateMap<Produto, ProdutoDto>();
            cfg.CreateMap<CadastrarProdutoDto, Produto>();
            cfg.CreateMap<AtualizarProdutoDto, Produto>();
        });

        _mapper = mapperConfig.CreateMapper();

        _produtosUseCase = new ProdutosUseCase(_produtosRepositoryMock.Object, _categoriaRepositoryMock.Object, _mapper);
        _produtosRepositoryMock.Setup(r => r.Atualizar(It.IsAny<Produto>())).ReturnsAsync((Produto p) => p);
        _produtosRepositoryMock.Setup(r => r.Inserir(It.IsAny<Produto>())).ReturnsAsync((Produto p) => p);
    }


    [When(@"o produto ""(.*)"" com valor ""(.*)"" é cadastrado na categoria ""(.*)""")]
    [When(@"o produto ""(.*)"" com valor ""(.*)"" � cadastrado na categoria ""(.*)""")]
    public async Task WhenOProdutoComValorECategoriaERegistrado(string produtoDescricao, decimal valor, string categoriaDescricao)
    {
        var cadastrarProdutoDto = new CadastrarProdutoDto
        {
            Descricao = produtoDescricao,
            Valor = valor,
            CategoriaId = 1 
        };

        try
        {
            _resultadoCadastro = await _produtosUseCase.Cadastrar(cadastrarProdutoDto);
        }
        catch (Exception ex)
        {
            _excecao = ex;
        }
    }

    [Then(@"o produto deve ser cadastrado com sucesso")]
    public void ThenOProdutoDeveSerCadastradoComSucesso()
    {
        _resultadoCadastro.Should().NotBeNull();
        _resultadoCadastro.Descricao.Should().Be("BigMc");
    }

    [Then(@"o produto deve ter a categoria ""(.*)""")]
    public void ThenOProdutoDeveTerACategoria(string categoriaDescricao)
    {
        _resultadoCadastro.Should().NotBeNull();
        _resultadoCadastro.Categoria.Descricao.Should().Be(categoriaDescricao);
    }

    [Given(@"a categoria ""(.*)"" não existe")]
    [Given(@"a categoria ""(.*)"" n�o existe")]
    public void GivenACategoriaNaoExiste(string categoriaDescricao)
    {
        _categoriaRepositoryMock.Setup(repo => repo.ObterPorId(It.IsAny<int>()))
              .ReturnsAsync((Categoria)null); 
    }

    [When(@"o produto ""(.*)"" com valor ""(.*)"" é cadastrado com uma categoria inválida")]
    [When(@"o produto ""(.*)"" com valor ""(.*)"" � cadastrado com uma categoria inv�lida")]
    public async Task WhenOProdutoComCategoriaInvalidaERegistrado(string produtoDescricao, decimal valor)
    {
        var cadastrarProdutoDto = new CadastrarProdutoDto
        {
            Descricao = produtoDescricao,
            Valor = valor,
            CategoriaId = 999 
        };

        try
        {
            _resultadoCadastro = await _produtosUseCase.Cadastrar(cadastrarProdutoDto);
        }
        catch (Exception ex)
        {
            _excecao = ex;
        }
    }

    [Then(@"uma exceção de categoria inválida deve ser lançada")]
    [Then(@"uma exce��o de categoria inv�lido deve ser lan�ada")]
    public void ThenUmaExcecaoDeCategoriaInvalidaDeveSerLancada()
    {
        _excecao.Should().NotBeNull();
        _excecao.Message.Should().Contain("Categoria inválida");
    }

    [Given(@"o produto ""(.*)"" existe com valor ""(.*)""")]
    public void GivenOProdutoExisteComValor(string descricao, decimal valor)
    {
        var produto = new Produto(1, descricao, valor, new Categoria { Id = 1, Descricao = "Lanche" });
        _produtosRepositoryMock.Setup(repo => repo.ObterPorId(1)).ReturnsAsync(produto);
    }

    [Given(@"a categoria ""(.*)"" existe")]
    public void GivenACategoriaExisteParaAtualizacao(string descricao)
    {
        _categoriaRepositoryMock.Setup(repo => repo.ObterPorId(1))
            .ReturnsAsync(new Categoria { Id = 1, Descricao = descricao });
    }

    [When(@"o produto ""(.*)"" é atualizado para o valor ""(.*)"" e categoria ""(.*)""")]
    [When(@"o produto ""(.*)"" � atualizado para o valor ""(.*)"" e categoria ""(.*)""")]
    public async Task WhenOProdutoAtualizadoParaOCategoriaEValor(string produtoDescricao, decimal valor, string categoriaDescricao)
    {
        var atualizarProdutoDto = new AtualizarProdutoDto
        {
            Descricao = produtoDescricao,
            Valor = valor,
            CategoriaId = 1
        };

        try
        {
            _produtoAtualizado = await _produtosUseCase.Atualizar(atualizarProdutoDto, 1);
        }
        catch (Exception ex)
        {
            _excecao = ex;
        }
    }

    [Then(@"o produto deve ser atualizado com sucesso")]
    public void ThenOProdutoDeveSerAtualizadoComSucesso()
    {
        _produtoAtualizado.Should().NotBeNull();
        _produtoAtualizado.Descricao.Should().Be("BigMc");
        _produtoAtualizado.Valor.Should().Be(20);
    }

    [Then(@"o produto deve ter o valor ""(.*)"" e a categoria ""(.*)""")]
    public void ThenOProdutoDeveTerOValorEACategoria(decimal valor, string categoriaDescricao)
    {
        _produtoAtualizado.Valor.Should().Be(valor);
        _produtoAtualizado.Categoria.Descricao.Should().Be(categoriaDescricao);
    }

    [Given(@"o produto ""(.*)"" existe")]
    public void GivenOProdutoExisteParaExclusao(string produtoDescricao)
    {
        var produto = new Produto(1, produtoDescricao, 15.00m, new Categoria { Id = 1, Descricao = "Lanche" });
        _produtosRepositoryMock.Setup(repo => repo.ObterPorId(1)).ReturnsAsync(produto);
    }

    [When(@"o produto ""(.*)"" é excluído")]
    [When(@"o produto ""(.*)"" � exclu�do")]
    public async Task WhenOProdutoEExcluido(string produtoDescricao)
    {
        try
        {
            await _produtosUseCase.Excluir(1);
        }
        catch (Exception ex)
        {
            _excecao = ex;
        }
    }

    [Then(@"o produto deve ser excluído com sucesso")]
    [Then(@"o produto deve ser exclu�do com sucesso")]
    public void ThenOProdutoDeveSerExcluidoComSucesso()
    {
        _excecao.Should().BeNull(); 
    }

    [When(@"o produto é consultado pelo ID ""(.*)""")]
    [When(@"o produto � consultado pelo ID ""(.*)""")]
    public async Task WhenOProdutoEConsultadoPeloID(int produtoId)
    {
        try
        {
            _produtoConsultado = await _produtosUseCase.Obter(produtoId);
            if (_produtoConsultado is null)
                throw new NullReferenceException("Produto Id inválido");
        }
        catch (Exception ex)
        {
            _excecao = ex;
        }
    }

    [Then(@"o produto com ID ""(.*)"" deve ser retornado")]
    public void ThenOProdutoComIDDeveSerRetornado(int produtoId)
    {
        _produtoConsultado.Should().NotBeNull();
        _produtoConsultado.Id.Should().Be(produtoId);
    }

    [Then(@"uma exceção de produto inválido deve ser lançada")]
    [Then(@"uma exce��o de produto inv�lido deve ser lan�ada")]
    public void ThenUmaExcecaoDeProdutoInvalidoDeveSerLancada()
    {
        _excecao.Should().NotBeNull();
        _excecao.Message.Should().Contain("Produto Id inválido");
    }

    [Given(@"existem produtos cadastrados")]
    public void GivenExistemProdutosCadastrados()
    {
        var produtos = new List<Produto>
        {
            new Produto(1, "BigMc", 15.00m, new Categoria { Id = 1, Descricao = "Lanche" }),
            new Produto(2, "McChicken", 18.00m, new Categoria { Id = 1, Descricao = "Lanche" })
        };

        _produtosRepositoryMock.Setup(repo => repo.Listar()).ReturnsAsync(produtos);
    }

    [When(@"a lista de produtos é consultada")]
    [When(@"a lista de produtos � consultada")]
    public async Task WhenAListaDeProdutosConsultada()
    {
        try
        {
            _resultadoLista = await _produtosUseCase.Listar();
        }
        catch (Exception ex)
        {
            _excecao = ex;
        }
    }

    [Then(@"a lista de produtos deve ser retornada")]
    public void ThenAListaDeProdutosDeveSerRetornada()
    {
        _resultadoLista.Should().NotBeNull();
        _resultadoLista.Should().HaveCountGreaterThan(0);
    }

    [Then(@"a lista de produtos deve conter os produtos cadastrados")]
    public void ThenAListaDeProdutosDeveConterOsProdutosCadastrados()
    {
        _resultadoLista.Should().Contain(p => p.Descricao == "BigMc");
        _resultadoLista.Should().Contain(p => p.Descricao == "McChicken");
    }

    [Given(@"o produto ""(.*)"" não existe")]
    [Given(@"o produto ""(.*)"" n�o existe")]
    public void GivenOProdutoNOExiste(string produtoDescricao)
    {
        _produtosRepositoryMock.Setup(repo => repo.ObterPorId(1))
            .ReturnsAsync((Produto)null);
    }

    [When(@"o produto ""(.*)"" é atualizado para uma categoria inválida")]
    [When(@"o produto ""(.*)"" � atualizado para uma categoria inv�lida")]

    public async Task WhenOProdutoAtualizadoParaUmaCategoriaInvLida(string produtoDescricao)
    {
        try
        {
            var atualizarProdutoDto = new AtualizarProdutoDto
            {
                Descricao = produtoDescricao,
                Valor = 1,
                CategoriaId = 999
            }; 
            
            await _produtosUseCase.Atualizar(atualizarProdutoDto, 1);
        }
        catch (Exception ex)
        {            
            _excecao = ex;
        }
    }

    [Then(@"uma exceção de categoria inválida deve ser lançada")]
    [Then(@"uma exce��o de categoria inv�lida deve ser lan�ada")]
    public void ThenUmaExceODeCategoriaInvLidaDeveSerLanAda()
    {        
        Assert.NotNull(_excecao);
        Assert.IsType<Exception>(_excecao);
        Assert.Equal("Categoria inválida", _excecao.Message); 
    }
}
