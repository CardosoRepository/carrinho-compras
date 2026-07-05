using CarrinhoCompras.Domain.Entities;

namespace CarrinhoCompras.Application.Abstractions.Persistence;

public interface ICupomRepository
{
    Task<Cupom?> ObterPorCodigoAsync(string codigoCupom, CancellationToken cancellationToken = default);
}