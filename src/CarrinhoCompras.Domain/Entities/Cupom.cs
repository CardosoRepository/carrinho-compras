using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Domain.Entities;

public sealed class Cupom
{
    public Guid Id { get; private set; }

    public string CodigoCupom { get; private set; } = string.Empty;

    public decimal PercentualDesconto { get; private set; }

    private Cupom()
    {
    }

    public Cupom(
        Guid id,
        string codigoCupom,
        decimal percentualDesconto)
    {
        if (id == Guid.Empty)
        {
            throw new RegraDeNegocioException("O identificador do cupom é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(codigoCupom))
        {
            throw new RegraDeNegocioException("O código do cupom é obrigatório.");
        }

        if (percentualDesconto <= 0 || percentualDesconto > 100)
        {
            throw new RegraDeNegocioException("O percentual de desconto deve estar entre 0 e 100.");
        }

        Id = id;
        CodigoCupom = codigoCupom.Trim().ToUpperInvariant();
        PercentualDesconto = percentualDesconto;
    }

    public decimal CalcularDesconto(decimal subtotal)
    {
        if (subtotal <= 0)
        {
            return 0;
        }

        var desconto = subtotal * PercentualDesconto / 100m;

        return Math.Round(
            desconto,
            2,
            MidpointRounding.AwayFromZero);
    }
}