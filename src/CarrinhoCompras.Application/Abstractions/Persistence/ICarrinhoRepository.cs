using CarrinhoCompras.Domain.Entities;

namespace CarrinhoCompras.Application.Abstractions.Persistence;

public interface ICarrinhoRepository
{
    Task<Carrinho?> ObterPorIdAsync(
        Guid carrinhoId,
        CancellationToken cancellationToken = default);

    Task<Carrinho?> ObterParaAtualizacaoAsync(
        Guid carrinhoId,
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Carrinho carrinho,
        CancellationToken cancellationToken = default);

    Task SalvarAlteracoesAsync(
        CancellationToken cancellationToken = default);
}