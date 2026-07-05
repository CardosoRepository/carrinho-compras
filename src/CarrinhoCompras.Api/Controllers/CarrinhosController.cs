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
    private readonly AdicionarItemUseCase _adicionarItemUseCase;
    private readonly AlterarQuantidadeItemUseCase _alterarQuantidadeItemUseCase;

    public CarrinhosController(
        CriarCarrinhoUseCase criarCarrinhoUseCase,
        ObterCarrinhoUseCase obterCarrinhoUseCase,
        AdicionarItemUseCase adicionarItemUseCase,
        AlterarQuantidadeItemUseCase alterarQuantidadeItemUseCase)
    {
        _criarCarrinhoUseCase = criarCarrinhoUseCase;
        _obterCarrinhoUseCase = obterCarrinhoUseCase;
        _adicionarItemUseCase = adicionarItemUseCase;
        _alterarQuantidadeItemUseCase = alterarQuantidadeItemUseCase;
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
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
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

    [HttpPost("{carrinhoId:guid}/itens")]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> AdicionarItem(
        Guid carrinhoId,
        [FromBody] AdicionarItemRequest request,
        CancellationToken cancellationToken)
    {
        var carrinho =
            await _adicionarItemUseCase.ExecutarAsync(
                carrinhoId,
                request,
                cancellationToken);

        return Ok(carrinho);
    }

    [HttpPut("{carrinhoId:guid}/itens/{produtoId:guid}")]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> AlterarQuantidadeItem(
        Guid carrinhoId,
        Guid produtoId,
        [FromBody] AlterarQuantidadeItemRequest request,
        CancellationToken cancellationToken)
    {
        var carrinho =
            await _alterarQuantidadeItemUseCase.ExecutarAsync(
                carrinhoId,
                produtoId,
                request,
                cancellationToken);

        return Ok(carrinho);
    }
}