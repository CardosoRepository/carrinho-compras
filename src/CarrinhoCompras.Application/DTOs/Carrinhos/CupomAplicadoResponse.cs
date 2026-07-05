namespace CarrinhoCompras.Application.DTOs.Carrinhos;

public sealed record CupomAplicadoResponse(
    Guid Id,
    string CodigoCupom,
    decimal PercentualDesconto);