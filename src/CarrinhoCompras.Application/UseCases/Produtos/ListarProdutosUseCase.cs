using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Produtos;

namespace CarrinhoCompras.Application.UseCases.Produtos;

public sealed class ListarProdutosUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public ListarProdutosUseCase(
        IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IReadOnlyCollection<ProdutoResponse>> ExecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var produtos = await _produtoRepository.ListarAsync(cancellationToken);

        return produtos
            .Select(produto =>
                new ProdutoResponse(
                    produto.Id,
                    produto.DescricaoProduto,
                    produto.QuantidadeEstoque,
                    produto.PrecoLiquido))
            .ToArray();
    }
}