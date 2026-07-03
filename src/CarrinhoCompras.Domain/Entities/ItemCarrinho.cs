using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Domain.Entities;

public sealed class ItemCarrinho
{
    public Guid Id { get; private set; }

    public Guid ProdutoId { get; private set; }

    public int Quantidade { get; private set; }

    public decimal PrecoUnitario { get; private set; }

    public decimal PrecoTotal =>
        PrecoUnitario * Quantidade;

    private ItemCarrinho()
    {
    }

    internal ItemCarrinho(
        Produto produto,
        int quantidade)
    {
        ArgumentNullException.ThrowIfNull(produto);

        produto.ValidarQuantidadeDisponivel(quantidade);

        Id = Guid.NewGuid();
        ProdutoId = produto.Id;
        Quantidade = quantidade;
        PrecoUnitario = produto.PrecoLiquido;
    }

    internal void SomarQuantidade(
        int quantidadeAdicionada,
        int quantidadeDisponivel)
    {
        if (quantidadeAdicionada <= 0)
        {
            throw new RegraDeNegocioException(
                "A quantidade adicionada deve ser maior que zero.");
        }

        var novaQuantidade =
            Quantidade + quantidadeAdicionada;

        ValidarEstoque(
            novaQuantidade,
            quantidadeDisponivel);

        Quantidade = novaQuantidade;
    }

    internal void AlterarQuantidade(
        int novaQuantidade,
        int quantidadeDisponivel)
    {
        if (novaQuantidade <= 0)
        {
            throw new RegraDeNegocioException(
                "A quantidade deve ser maior que zero.");
        }

        ValidarEstoque(
            novaQuantidade,
            quantidadeDisponivel);

        Quantidade = novaQuantidade;
    }

    private static void ValidarEstoque(
        int quantidade,
        int quantidadeDisponivel)
    {
        if (quantidade > quantidadeDisponivel)
        {
            throw new RegraDeNegocioException(
                $"Estoque insuficiente. Quantidade disponível: {quantidadeDisponivel}.");
        }
    }
}