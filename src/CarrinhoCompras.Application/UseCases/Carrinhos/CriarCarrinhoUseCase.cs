using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Mappings;
using CarrinhoCompras.Domain.Entities;

namespace CarrinhoCompras.Application.UseCases.Carrinhos;

public sealed class CriarCarrinhoUseCase
{
    private readonly ICarrinhoRepository _carrinhoRepository;

    public CriarCarrinhoUseCase(
        ICarrinhoRepository carrinhoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
    }

    public async Task<CarrinhoResponse> ExecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var carrinho = new Carrinho();

        await _carrinhoRepository.AdicionarAsync(
            carrinho,
            cancellationToken);

        await _carrinhoRepository.SalvarAlteracoesAsync(
            cancellationToken);

        return CarrinhoResponseFactory.Criar(
            carrinho,
            Array.Empty<Produto>());
    }
}