using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarrinhoCompras.Infrastructure.Persistence.Configurations;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(produto => produto.Id);

        builder.Property(produto => produto.Id)
            .ValueGeneratedNever();

        builder.Property(produto => produto.DescricaoProduto)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(produto => produto.QuantidadeEstoque)
            .IsRequired();

        builder.Property(produto => produto.PrecoLiquido)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}