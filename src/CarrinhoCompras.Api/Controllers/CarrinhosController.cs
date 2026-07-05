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
    private readonly RemoverItemUseCase _removerItemUseCase;
    private readonly AplicarCupomUseCase _aplicarCupomUseCase;
    private readonly RemoverCupomUseCase _removerCupomUseCase;
    private readonly FinalizarCarrinhoUseCase _finalizarCarrinhoUseCase;


    public CarrinhosController(
        CriarCarrinhoUseCase criarCarrinhoUseCase,
        ObterCarrinhoUseCase obterCarrinhoUseCase,
        AdicionarItemUseCase adicionarItemUseCase,
        AlterarQuantidadeItemUseCase alterarQuantidadeItemUseCase,
        RemoverItemUseCase removerItemUseCase,
        AplicarCupomUseCase aplicarCupomUseCase,
        RemoverCupomUseCase removerCupomUseCase,
        FinalizarCarrinhoUseCase finalizarCarrinhoUseCase)
    {
        _criarCarrinhoUseCase = criarCarrinhoUseCase;
        _obterCarrinhoUseCase = obterCarrinhoUseCase;
        _adicionarItemUseCase = adicionarItemUseCase;
        _alterarQuantidadeItemUseCase = alterarQuantidadeItemUseCase;
        _removerItemUseCase = removerItemUseCase;
        _aplicarCupomUseCase = aplicarCupomUseCase;
        _removerCupomUseCase = removerCupomUseCase;
        _finalizarCarrinhoUseCase = finalizarCarrinhoUseCase;
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

    [HttpDelete("{carrinhoId:guid}/itens/{produtoId:guid}")]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> RemoverItem(
        Guid carrinhoId,
        Guid produtoId,
        CancellationToken cancellationToken)
    {
        var carrinho =
            await _removerItemUseCase.ExecutarAsync(
                carrinhoId,
                produtoId,
                cancellationToken);

        return Ok(carrinho);
    }

    [HttpPut("{carrinhoId:guid}/cupom")]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> AplicarCupom(
        Guid carrinhoId,
        [FromBody] AplicarCupomRequest request,
        CancellationToken cancellationToken)
    {
        var carrinho = await _aplicarCupomUseCase.ExecutarAsync(carrinhoId, request, cancellationToken);
        return Ok(carrinho);
    }

    [HttpDelete("{carrinhoId:guid}/cupom")]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> RemoverCupom(
        Guid carrinhoId,
        CancellationToken cancellationToken)
    {
        var carrinho = await _removerCupomUseCase.ExecutarAsync(carrinhoId, cancellationToken);
        return Ok(carrinho);
    }

    [HttpPost("{carrinhoId:guid}/finalizar")]
    [ProducesResponseType(
        typeof(CarrinhoResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarrinhoResponse>> Finalizar(
        Guid carrinhoId,
        CancellationToken cancellationToken)
    {
        var carrinho =
            await _finalizarCarrinhoUseCase.ExecutarAsync(
                carrinhoId,
                cancellationToken);

        return Ok(carrinho);
    }
}