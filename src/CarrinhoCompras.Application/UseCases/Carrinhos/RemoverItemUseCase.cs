using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.Mappings;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class RemoverItemUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public RemoverItemUseCase(
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<CarrinhoResponse> ExecutarAsync(
        Guid carrinhoId,
        Guid produtoId,
        CancellationToken cancellationToken = default)
    {
        var carrinho =
            await _carrinhoRepository.ObterParaAtualizacaoAsync(
                carrinhoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O carrinho '{carrinhoId}' não foi encontrado.");

        carrinho.RemoverProduto(produtoId);

        await _carrinhoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        var produtoIds =
            carrinho.Itens
                .Select(item => item.ProdutoId)
                .Distinct();

        var produtos =
            await _produtoRepository.ObterPorIdsAsync(
                produtoIds,
                cancellationToken);

        return CarrinhoResponseFactory.Criar(
            carrinho,
            produtos);
    }
}