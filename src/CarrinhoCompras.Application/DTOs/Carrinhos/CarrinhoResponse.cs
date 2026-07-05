namespace CarrinhoCompras.Application.DTOs.Carrinhos;

public sealed record CarrinhoResponse(
    Guid Id,
    string Status,
    IReadOnlyCollection<ItemCarrinhoResponse> Itens,
    CupomAplicadoResponse? CupomAplicado,
    decimal Subtotal,
    decimal Desconto,
    decimal Total);