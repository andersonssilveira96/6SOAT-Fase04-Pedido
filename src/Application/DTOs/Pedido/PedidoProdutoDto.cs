namespace Application.DTOs.Pedido
{
    public class PedidoProdutoDto : PedidoProdutoBaseDto
    {
        public string Nome { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => ValorUnitario * Quantidade;
    }
}
