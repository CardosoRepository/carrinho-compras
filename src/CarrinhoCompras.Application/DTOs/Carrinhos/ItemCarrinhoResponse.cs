namespace CarrinhoCompras.Application.DTOs.Carrinhos;

public sealed record ItemCarrinhoResponse(
    Guid ProdutoId,
    string DescricaoProduto,
    int Quantidade,
    decimal PrecoLiquidoUnitario,
    decimal PrecoTotal,
    int QuantidadeDisponivelEstoque);