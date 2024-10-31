namespace Domain.Entities
{
    public class Produto
    {
        public Produto() 
        {          
        }        
        public Produto(long id, string descricao, decimal valor, Categoria categoria)
        {
            Id = id;
            Descricao = descricao;
            Valor = valor;  
            Categoria = categoria;  
        }
        public long Id { get; private set; }    
        public string Descricao { get; private set; }
        public decimal Valor { get; private set; }
        public Categoria Categoria { get; private set; }
        public void AdicionarCategoria(Categoria categoria) => Categoria = categoria;
    }
}
