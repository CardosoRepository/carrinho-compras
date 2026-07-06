using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.Mappings;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class FinalizarCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public FinalizarCarrinhoUseCase(
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<CarrinhoResponse> ExecutarAsync(
        Guid carrinhoId,
        CancellationToken cancellationToken = default)
    {
        var carrinho =
            await _carrinhoRepository.ObterParaAtualizacaoAsync(
                carrinhoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O carrinho '{carrinhoId}' não foi encontrado.");

        carrinho.Finalizar();

        await _carrinhoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return await CarrinhoResponseFactory.CriarAsync(
            carrinho,
            _produtoRepository,
            cancellationToken);
    }
}