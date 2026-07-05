using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.UseCases.Carrinhos;

using Microsoft.AspNetCore.Mvc;

namespace CarrinhoCompras.Api.Controllers;

[ApiController]
[Route("api/carrinhos")]
public sealed class CarrinhosController : ControllerBase
{
    private readonly CriarCarrinhoUseCase _criarCarrinhoUseCase;
    private readonly ObterCarrinhoUseCase _obterCarrinhoUseCase;

    public CarrinhosController(
        CriarCarrinhoUseCase criarCarrinhoUseCase,
        ObterCarrinhoUseCase obterCarrinhoUseCase)
    {
        _criarCarrinhoUseCase = criarCarrinhoUseCase;
        _obterCarrinhoUseCase = obterCarrinhoUseCase;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status201Created)]
    public async Task<ActionResult<CarrinhoResponse>> Criar(
        CancellationToken cancellationToken)
    {
        var carrinho =
            await _criarCarrinhoUseCase.ExecutarAsync(
                cancellationToken);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { carrinhoId = carrinho.Id },
            carrinho);
    }

    [HttpGet("{carrinhoId:guid}")]
    [ProducesResponseType(typeof(CarrinhoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> ObterPorId(
        Guid carrinhoId,
        CancellationToken cancellationToken)
    {
        var carrinho =
            await _obterCarrinhoUseCase.ExecutarAsync(
                carrinhoId,
                cancellationToken);

        return Ok(carrinho);
    }
}