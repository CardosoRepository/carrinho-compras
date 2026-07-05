using CarrinhoCompras.Domain.Entities;

namespace CarrinhoCompras.Application.Abstractions.Persistence;

public interface IProdutoRepository
{
    Task<IReadOnlyCollection<Produto>> ListarAsync(
        CancellationToken cancellationToken = default);

    Task<Produto?> ObterPorIdAsync(
        Guid produtoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Produto>> ObterPorIdsAsync(
        IEnumerable<Guid> produtoIds,
        CancellationToken cancellationToken = default);
}