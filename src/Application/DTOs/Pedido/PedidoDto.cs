namespace Application.DTOs.Pedido
{
    public class PedidoDto
    {
        public long Id { get; set; }
        public bool Viagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public long? ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; }
        public virtual ICollection<PedidoProdutoDto> Produtos { get; set; }
    }
}
