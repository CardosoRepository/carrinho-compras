using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.Mappings;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class AdicionarItemUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public AdicionarItemUseCase(
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<CarrinhoResponse> ExecutarAsync(
        Guid carrinhoId,
        AdicionarItemRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.ProdutoId == Guid.Empty)
        {
            throw new RegraDeNegocioException(
                "O identificador do produto é obrigatório.");
        }

        var carrinho =
            await _carrinhoRepository.ObterParaAtualizacaoAsync(
                carrinhoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O carrinho '{carrinhoId}' não foi encontrado.");

        var produto =
            await _produtoRepository.ObterPorIdAsync(
                request.ProdutoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O produto '{request.ProdutoId}' não foi encontrado.");

        carrinho.AdicionarProduto(
            produto,
            request.Quantidade);

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