using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarrinhoCompras.Infrastructure.Persistence.Configurations;

public sealed class CarrinhoConfiguration : IEntityTypeConfiguration<Carrinho>
{
    public void Configure(EntityTypeBuilder<Carrinho> builder)
    {
        builder.ToTable("Carrinhos");

        builder.HasKey(carrinho => carrinho.Id);

        builder.Property(carrinho => carrinho.Id)
            .ValueGeneratedNever();

        builder.Property(carrinho => carrinho.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(carrinho => carrinho.CupomAplicado)
            .WithMany()
            .HasForeignKey(carrinho => carrinho.CupomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(carrinho => carrinho.Itens)
            .WithOne()
            .HasForeignKey("CarrinhoId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(carrinho => carrinho.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(carrinho => carrinho.Subtotal);
        builder.Ignore(carrinho => carrinho.Desconto);
        builder.Ignore(carrinho => carrinho.Total);
    }
}