using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.Mappings;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class AplicarCupomUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICupomRepository _cupomRepository;

    public AplicarCupomUseCase(
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository,
        ICupomRepository cupomRepository)
    {
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
        _cupomRepository = cupomRepository;
    }

    public async Task<CarrinhoResponse> ExecutarAsync(
        Guid carrinhoId,
        AplicarCupomRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(
            request.CodigoCupom))
        {
            throw new RegraDeNegocioException(
                "O código do cupom é obrigatório.");
        }

        var carrinho =
            await _carrinhoRepository.ObterParaAtualizacaoAsync(
                carrinhoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O carrinho '{carrinhoId}' não foi encontrado.");

        var codigoCupom =
            request.CodigoCupom
                .Trim()
                .ToUpperInvariant();

        var cupom =
            await _cupomRepository.ObterPorCodigoAsync(
                codigoCupom,
                cancellationToken)
            ?? throw new RegraDeNegocioException(
                "O cupom informado é inválido.");

        carrinho.AplicarCupom(cupom);

        await _carrinhoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return await CarrinhoResponseFactory.CriarAsync(
            carrinho,
            _produtoRepository,
            cancellationToken);
    }
}