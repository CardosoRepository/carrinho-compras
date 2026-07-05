namespace CarrinhoCompras.Application.DTOs.Carrinhos;

public sealed class AdicionarItemRequest
{
    public Guid ProdutoId { get; init; }

    public int Quantidade { get; init; } = 1;
}