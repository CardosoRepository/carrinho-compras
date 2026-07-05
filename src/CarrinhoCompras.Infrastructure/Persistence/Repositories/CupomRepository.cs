using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CarrinhoCompras.Infrastructure.Persistence.Repositories;

public sealed class CupomRepository : ICupomRepository
{
    private readonly CarrinhoComprasDbContext _context;
    public CupomRepository(CarrinhoComprasDbContext context)
    {
        _context = context;
    }

    public async Task<Cupom?> ObterPorCodigoAsync(string codigoCupom, CancellationToken cancellationToken = default)
    {
        var codigoNormalizado = codigoCupom
            .Trim()
            .ToUpperInvariant();

        return await _context.Cupons.SingleOrDefaultAsync(cupom => cupom.CodigoCupom == codigoNormalizado, cancellationToken);
    }
}