namespace CarrinhoCompras.Application.DTOs.Produtos;

public sealed record ProdutoResponse(
    Guid Id,
    string DescricaoProduto,
    int QuantidadeEstoque,
    decimal PrecoLiquido);