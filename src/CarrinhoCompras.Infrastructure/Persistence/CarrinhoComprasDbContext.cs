using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CarrinhoCompras.Infrastructure.Persistence;

public sealed class CarrinhoComprasDbContext : DbContext
{
    public CarrinhoComprasDbContext(
        DbContextOptions<CarrinhoComprasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<Cupom> Cupons => Set<Cupom>();

    public DbSet<Carrinho> Carrinhos => Set<Carrinho>();

    public DbSet<ItemCarrinho> ItensCarrinho => Set<ItemCarrinho>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CarrinhoComprasDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}