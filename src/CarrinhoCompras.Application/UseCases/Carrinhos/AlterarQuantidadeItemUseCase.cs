using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.Mappings;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class AlterarQuantidadeItemUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public AlterarQuantidadeItemUseCase(
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<CarrinhoResponse> ExecutarAsync(
        Guid carrinhoId,
        Guid produtoId,
        AlterarQuantidadeItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var carrinho =
            await _carrinhoRepository.ObterParaAtualizacaoAsync(
                carrinhoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O carrinho '{carrinhoId}' não foi encontrado.");

        var produto =
            await _produtoRepository.ObterPorIdAsync(
                produtoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O produto '{produtoId}' não foi encontrado.");

        carrinho.AlterarQuantidade(
            produto,
            request.Quantidade);

        await _carrinhoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return await CarrinhoResponseFactory.CriarAsync(
            carrinho,
            _produtoRepository,
            cancellationToken);
    }
}