using CarrinhoCompras.Domain.Enums;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Domain.Entities;

public sealed class Carrinho
{
    private readonly List<ItemCarrinho> _itens = [];

    public Guid Id { get; private set; }

    public StatusCarrinho Status { get; private set; }

    public Guid? CupomId { get; private set; }

    public Cupom? CupomAplicado { get; private set; }

    public IReadOnlyCollection<ItemCarrinho> Itens =>
        _itens.AsReadOnly();

    public decimal Subtotal =>
        _itens.Sum(item => item.PrecoTotal);

    public decimal Desconto =>
        CupomAplicado?.CalcularDesconto(Subtotal) ?? 0m;

    public decimal Total =>
        Subtotal - Desconto;

    public Carrinho()
    {
        Id = Guid.NewGuid();
        Status = StatusCarrinho.Aberto;
    }

    public void AdicionarProduto(
        Produto produto,
        int quantidade)
    {
        ArgumentNullException.ThrowIfNull(produto);

        GarantirQueEstaAberto();

        var itemExistente = _itens.SingleOrDefault(
            item => item.ProdutoId == produto.Id);

        if (itemExistente is null)
        {
            _itens.Add(new ItemCarrinho(produto, quantidade));
            return;
        }

        itemExistente.SomarQuantidade(
            quantidade,
            produto.QuantidadeEstoque);
    }

    public void AlterarQuantidade(
        Produto produto,
        int novaQuantidade)
    {
        ArgumentNullException.ThrowIfNull(produto);

        GarantirQueEstaAberto();

        var item = ObterItem(produto.Id);

        item.AlterarQuantidade(
            novaQuantidade,
            produto.QuantidadeEstoque);
    }

    public void RemoverProduto(Guid produtoId)
    {
        GarantirQueEstaAberto();

        var item = ObterItem(produtoId);

        _itens.Remove(item);
    }

    public void AplicarCupom(Cupom cupom)
    {
        ArgumentNullException.ThrowIfNull(cupom);

        GarantirQueEstaAberto();

        CupomId = cupom.Id;
        CupomAplicado = cupom;
    }

    public void RemoverCupom()
    {
        GarantirQueEstaAberto();

        CupomId = null;
        CupomAplicado = null;
    }

    public void Finalizar()
    {
        GarantirQueEstaAberto();

        Status = StatusCarrinho.Finalizado;
    }

    private ItemCarrinho ObterItem(Guid produtoId)
    {
        var item = _itens.SingleOrDefault(
            itemCarrinho => itemCarrinho.ProdutoId == produtoId);

        if (item is null)
        {
            throw new RegraDeNegocioException(
                "O produto não está presente no carrinho.");
        }

        return item;
    }

    private void GarantirQueEstaAberto()
    {
        if (Status == StatusCarrinho.Finalizado)
        {
            throw new RegraDeNegocioException(
                "O carrinho está finalizado e não pode ser alterado.");
        }
    }
}