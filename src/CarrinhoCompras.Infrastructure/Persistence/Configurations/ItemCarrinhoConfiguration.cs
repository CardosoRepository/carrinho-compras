using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarrinhoCompras.Infrastructure.Persistence.Configurations;

public sealed class ItemCarrinhoConfiguration : IEntityTypeConfiguration<ItemCarrinho>
{
    public void Configure(EntityTypeBuilder<ItemCarrinho> builder)
    {
        builder.ToTable("ItensCarrinho");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Property<Guid>("CarrinhoId")
            .IsRequired();

        builder.Property(item => item.ProdutoId)
            .IsRequired();

        builder.Property(item => item.Quantidade)
            .IsRequired();

        builder.Property(item => item.PrecoUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(item => item.PrecoTotal);

        builder.HasOne<Produto>()
            .WithMany()
            .HasForeignKey(item => item.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex("CarrinhoId", nameof(ItemCarrinho.ProdutoId))
            .IsUnique();
    }
}