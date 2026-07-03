using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Domain.Entities;

public sealed class Produto
{
    public Guid Id { get; private set; }

    public string DescricaoProduto { get; private set; } = string.Empty;

    public int QuantidadeEstoque { get; private set; }

    public decimal PrecoLiquido { get; private set; }

    private Produto()
    {
    }

    public Produto(
        Guid id,
        string descricaoProduto,
        int quantidadeEstoque,
        decimal precoLiquido)
    {
        if (id == Guid.Empty)
        {
            throw new RegraDeNegocioException("O identificador do produto é obrigatório");
        }

        if (string.IsNullOrWhiteSpace(descricaoProduto))
        {
            throw new RegraDeNegocioException("A descrição do produto é obrigatória.");
        }

        if (quantidadeEstoque < 0)
        {
            throw new RegraDeNegocioException("A quantidade em estoque não pode ser negativa.");
        }

        if (precoLiquido < 0)
        {
            throw new RegraDeNegocioException("O preço líquido não pode ser negativo.");
        }

        Id = id;
        DescricaoProduto = descricaoProduto.Trim();
        QuantidadeEstoque = quantidadeEstoque;
        PrecoLiquido = precoLiquido;
    }

    public void ValidarQuantidadeDisponivel(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new RegraDeNegocioException("A quantidade deve ser maior que zero.");
        }

        if (quantidade > QuantidadeEstoque)
        {
            throw new RegraDeNegocioException($"Estoque insuficiente. Quantidade disponível: {QuantidadeEstoque}.");
        }
    }
}