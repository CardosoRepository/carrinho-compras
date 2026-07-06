using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Domain.Entities;

namespace CarrinhoCompras.Application.Mappings;

internal static class CarrinhoResponseFactory
{
    public static async Task<CarrinhoResponse> CriarAsync(
        Carrinho carrinho,
        IProdutoRepository produtoRepository,
        CancellationToken cancellationToken = default)
    {
        var produtoIds =
            carrinho.Itens
                .Select(item => item.ProdutoId)
                .Distinct()
                .ToArray();

        IReadOnlyCollection<Produto> produtos =
            produtoIds.Length == 0
                ? Array.Empty<Produto>()
                : await produtoRepository.ObterPorIdsAsync(
                    produtoIds,
                    cancellationToken);

        return Criar(
            carrinho,
            produtos);
    }

    public static CarrinhoResponse Criar(
        Carrinho carrinho,
        IReadOnlyCollection<Produto> produtos)
    {
        var produtosPorId =
            produtos.ToDictionary(
                produto => produto.Id);

        var itens =
            carrinho.Itens
                .Select(item =>
                {
                    if (
                        !produtosPorId.TryGetValue(
                            item.ProdutoId,
                            out var produto)
                    )
                    {
                        throw new InvalidOperationException(
                            $"O produto '{item.ProdutoId}' associado ao carrinho não foi encontrado.");
                    }

                    return new ItemCarrinhoResponse(
                        item.ProdutoId,
                        produto.DescricaoProduto,
                        item.Quantidade,
                        item.PrecoUnitario,
                        item.PrecoTotal,
                        produto.QuantidadeEstoque);
                })
                .OrderBy(
                    item => item.DescricaoProduto)
                .ToArray();

        var cupomAplicado =
            carrinho.CupomAplicado is null
                ? null
                : new CupomAplicadoResponse(
                    carrinho.CupomAplicado.Id,
                    carrinho.CupomAplicado.CodigoCupom,
                    carrinho.CupomAplicado
                        .PercentualDesconto);

        return new CarrinhoResponse(
            carrinho.Id,
            carrinho.Status.ToString(),
            itens,
            cupomAplicado,
            carrinho.Subtotal,
            carrinho.Desconto,
            carrinho.Total);
    }
}