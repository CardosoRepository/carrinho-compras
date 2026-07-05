using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CarrinhoCompras.Infrastructure.Persistence.Repositories;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly CarrinhoComprasDbContext _context;

    public ProdutoRepository(CarrinhoComprasDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Produto>> ListarAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(produto => produto.DescricaoProduto)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Produto?> ObterPorIdAsync(
        Guid produtoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .AsNoTracking()
            .SingleOrDefaultAsync(
                produto => produto.Id == produtoId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<Produto>> ObterPorIdsAsync(
        IEnumerable<Guid> produtoIds,
        CancellationToken cancellationToken = default)
    {
        var ids =
            produtoIds
                .Distinct()
                .ToArray();

        if (ids.Length == 0)
        {
            return Array.Empty<Produto>();
        }

        return await _context.Produtos
            .AsNoTracking()
            .Where(produto => ids.Contains(produto.Id))
            .ToArrayAsync(cancellationToken);
    }
}