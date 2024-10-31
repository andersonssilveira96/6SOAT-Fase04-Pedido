using Application.DTOs.Produtos;
using Domain.Repositories;
using AutoMapper;
using Domain.Entities;

namespace Application.UseCase.Produtos
{
    public class ProdutosUseCase : IProdutosUseCase
    {
        private readonly IProdutosRepository _produtosRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public ProdutosUseCase(IProdutosRepository produtosRepository, ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _produtosRepository = produtosRepository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        public async Task<ProdutoDto> Obter(long id) => _mapper.Map<ProdutoDto>(await _produtosRepository.ObterPorId(id));     
        public async Task<List<ProdutoDto>> Listar() => _mapper.Map<List<ProdutoDto>>(await _produtosRepository.Listar());       
        public async Task<List<ProdutoDto>> ListarPorCategoria(long idCategoria) => _mapper.Map<List<ProdutoDto>>(await _produtosRepository.ListarPorCategoria(idCategoria));    
        public async Task<ProdutoDto> Cadastrar(CadastrarProdutoDto cadastrarProdutoDto)
        {
            var produto = _mapper.Map<Produto>(cadastrarProdutoDto);
            
            var categoria = await _categoriaRepository.ObterPorId(cadastrarProdutoDto.CategoriaId);

            if (categoria is null) 
                throw new Exception("Categoria inválida");

            produto.AdicionarCategoria(categoria);

            return _mapper.Map<ProdutoDto>(await _produtosRepository.Inserir(produto));
        }

        public async Task<ProdutoDto> Atualizar(AtualizarProdutoDto atualizarProdutoDto, long id)
        {
            var categoria = await _categoriaRepository.ObterPorId(atualizarProdutoDto.CategoriaId);

            if (categoria is null)
                throw new Exception("Categoria inválida");

            var produto = new Produto(id, atualizarProdutoDto.Descricao, atualizarProdutoDto.Valor, categoria);                      

            return _mapper.Map<ProdutoDto>(await _produtosRepository.Atualizar(produto));
        }

        public async Task Excluir(long id)
        {
            var produto = await _produtosRepository.ObterPorId(id);

            if (produto is null)
                throw new Exception("Produto Id inválido");

            await _produtosRepository.Excluir(produto);
        }
    }
}
