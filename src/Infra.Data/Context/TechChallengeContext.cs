using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Domain.Enums;

namespace Infra.Data.Context
{
    public class TechChallengeContext : DbContext
    {
        public TechChallengeContext()
        {            
        }
        public TechChallengeContext(DbContextOptions<TechChallengeContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Categoria> Categoria { get; set; } 
        public virtual DbSet<Pedido> Pedido { get; set; }
        public virtual DbSet<PedidoProduto> PedidoProduto { get; set; }
        public virtual DbSet<Produto> Produto { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>()
               .HasData(
                    new Categoria { Id = CategoriaEnum.Lanche.GetHashCode(), Descricao = nameof(CategoriaEnum.Lanche) },
                    new Categoria { Id = CategoriaEnum.Acompanhamento.GetHashCode(), Descricao = nameof(CategoriaEnum.Acompanhamento) },
                    new Categoria { Id = CategoriaEnum.Bebida.GetHashCode(), Descricao = nameof(CategoriaEnum.Bebida) },
                    new Categoria { Id = CategoriaEnum.Sobremesa.GetHashCode(), Descricao = nameof(CategoriaEnum.Sobremesa) }
               );

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());         
        }
    }
}
