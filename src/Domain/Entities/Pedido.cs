using Domain.Enums;

namespace Domain.Entities
{
    public class Pedido
    {

        public Pedido()
        {
            
        }
        public Pedido(long id, StatusEnum status)
        {
            Id = id;
            Status = status;
        }
        public Pedido(long? clienteid, ICollection<PedidoProduto> pedidoProdutos, bool viagem = false)
        {
            ClienteId = clienteid;
            DataCriacao = DateTime.Now;
            Status = StatusEnum.PagamentoPendente;
            Produtos = pedidoProdutos;
            ValorTotal = pedidoProdutos.Sum(x => x.Quantidade * x.Produto.Valor);
            Viagem = viagem;
        }
        public void AtualizarStatus(StatusEnum status) => Status = status;
        public long Id { get; private set; }
        public bool Viagem { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public long? ClienteId { get; private set; }
        public decimal ValorTotal { get; private set; }
        public StatusEnum Status { get; private set; }
        public virtual ICollection<PedidoProduto> Produtos { get; private set; }
    }
}
