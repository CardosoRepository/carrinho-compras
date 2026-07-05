using CarrinhoCompras.Application.DTOs.Produtos;
using CarrinhoCompras.Application.UseCases.Produtos;

using Microsoft.AspNetCore.Mvc;

namespace CarrinhoCompras.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public sealed class ProdutosController : ControllerBase
{
    private readonly ListarProdutosUseCase _listarProdutosUseCase;

    public ProdutosController(
        ListarProdutosUseCase listarProdutosUseCase)
    {
        _listarProdutosUseCase = listarProdutosUseCase;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<ProdutoResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProdutoResponse>>> Listar(
        CancellationToken cancellationToken)
    {
        var produtos =
            await _listarProdutosUseCase.ExecutarAsync(
                cancellationToken);

        return Ok(produtos);
    }
}