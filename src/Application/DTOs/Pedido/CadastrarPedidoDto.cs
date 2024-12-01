namespace Application.DTOs.Pedido
{
    public class CadastrarPedidoDto
    {
        public long? ClienteId { get; set; }
        public virtual List<PedidoProdutoBaseDto> Produtos { get; set; }
        public bool Viagem { get; set; }
    }
}
