namespace Application.DTOs.Pedido
{
    public class PedidoProdutoBaseDto
    {
        public long ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
    }
}
