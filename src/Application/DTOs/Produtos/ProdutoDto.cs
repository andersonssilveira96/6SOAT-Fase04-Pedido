namespace Application.DTOs.Produtos
{
    public class ProdutoDto : ProdutoBaseDto
    {
        public long Id { get; set; }
        public CategoriaDto Categoria { get; set; }
    }
}
