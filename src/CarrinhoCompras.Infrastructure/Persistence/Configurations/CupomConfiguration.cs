using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarrinhoCompras.Infrastructure.Persistence.Configurations;

public sealed class CupomConfiguration : IEntityTypeConfiguration<Cupom>
{
    public void Configure(EntityTypeBuilder<Cupom> builder)
    {
        builder.ToTable("Cupons");

        builder.HasKey(cupom => cupom.Id);

        builder.Property(cupom => cupom.Id)
            .ValueGeneratedNever();

        builder.Property(cupom => cupom.CodigoCupom)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(cupom => cupom.PercentualDesconto)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.HasIndex(cupom => cupom.CodigoCupom)
            .IsUnique();
    }
}