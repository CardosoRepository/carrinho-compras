using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.Mappings;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class ObterCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public ObterCarrinhoUseCase(
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
            await _carrinhoRepository.ObterPorIdAsync(
                carrinhoId,
                cancellationToken)
            ?? throw new RecursoNaoEncontradoException(
                $"O carrinho '{carrinhoId}' não foi encontrado.");

        return await CarrinhoResponseFactory.CriarAsync(
            carrinho,
            _produtoRepository,
            cancellationToken);
    }
}