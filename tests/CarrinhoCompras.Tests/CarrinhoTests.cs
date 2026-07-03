using CarrinhoCompras.Domain.Entities;
using CarrinhoCompras.Domain.Enums;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Tests;

public sealed class CarrinhoTests
{
    [Fact]
    public void AdicionarProduto_DeveAdicionarNovoItem()
    {
        var produto = CriarProduto(
            quantidadeEstoque: 10,
            precoLiquido: 100m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);

        var item = Assert.Single(carrinho.Itens);

        Assert.Equal(produto.Id, item.ProdutoId);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(100m, item.PrecoUnitario);
        Assert.Equal(200m, item.PrecoTotal);
        Assert.Equal(200m, carrinho.Subtotal);
        Assert.Equal(200m, carrinho.Total);
    }

    [Fact]
    public void AdicionarProdutoExistente_DeveSomarQuantidade()
    {
        var produto = CriarProduto(10, 25m);
        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AdicionarProduto(produto, 3);

        var item = Assert.Single(carrinho.Itens);

        Assert.Equal(5, item.Quantidade);
        Assert.Equal(125m, item.PrecoTotal);
    }

    [Fact]
    public void AlterarQuantidade_DeveSubstituirQuantidadeAnterior()
    {
        var produto = CriarProduto(10, 30m);
        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AlterarQuantidade(produto, 5);

        var item = Assert.Single(carrinho.Itens);

        Assert.Equal(5, item.Quantidade);
        Assert.Equal(150m, item.PrecoTotal);
    }

    [Fact]
    public void AdicionarQuantidadeAcimaDoEstoque_DeveLancarExcecao()
    {
        var produto = CriarProduto(3, 50m);
        var carrinho = new Carrinho();

        var excecao = Assert.Throws<RegraDeNegocioException>(
            () => carrinho.AdicionarProduto(produto, 4));

        Assert.Contains(
            "Estoque insuficiente",
            excecao.Message);
    }

    [Fact]
    public void AplicarCupom_DeveCalcularDescontoETotal()
    {
        var produto = CriarProduto(10, 100m);

        var cupom = new Cupom(
            Guid.NewGuid(),
            "10OFF",
            10m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AplicarCupom(cupom);

        Assert.Equal(200m, carrinho.Subtotal);
        Assert.Equal(20m, carrinho.Desconto);
        Assert.Equal(180m, carrinho.Total);
    }

    [Fact]
    public void AplicarOutroCupom_DeveSubstituirCupomAnterior()
    {
        var produto = CriarProduto(10, 100m);

        var cupom10 = new Cupom(
            Guid.NewGuid(),
            "10OFF",
            10m);

        var cupom15 = new Cupom(
            Guid.NewGuid(),
            "15OFF",
            15m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AplicarCupom(cupom10);
        carrinho.AplicarCupom(cupom15);

        Assert.NotNull(carrinho.CupomAplicado);
        Assert.Equal(cupom15.Id, carrinho.CupomAplicado.Id);
        Assert.Equal(30m, carrinho.Desconto);
        Assert.Equal(170m, carrinho.Total);
    }

    [Fact]
    public void RemoverProduto_DeveAtualizarTotais()
    {
        var primeiroProduto = CriarProduto(10, 100m);
        var segundoProduto = CriarProduto(10, 50m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(primeiroProduto, 1);
        carrinho.AdicionarProduto(segundoProduto, 2);

        carrinho.RemoverProduto(primeiroProduto.Id);

        Assert.Single(carrinho.Itens);
        Assert.Equal(100m, carrinho.Subtotal);
    }

    [Fact]
    public void FinalizarCarrinho_DeveBloquearAlteracoes()
    {
        var produto = CriarProduto(10, 100m);
        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 1);
        carrinho.Finalizar();

        Assert.Equal(
            StatusCarrinho.Finalizado,
            carrinho.Status);

        var excecao = Assert.Throws<RegraDeNegocioException>(
            () => carrinho.AdicionarProduto(produto, 1));

        Assert.Contains(
            "finalizado",
            excecao.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RemoverProdutoInexistente_DeveLancarExcecao()
    {
        var carrinho = new Carrinho();

        var excecao = Assert.Throws<RegraDeNegocioException>(
            () => carrinho.RemoverProduto(Guid.NewGuid()));

        Assert.Contains(
            "não está presente",
            excecao.Message);
    }

    private static Produto CriarProduto(
        int quantidadeEstoque,
        decimal precoLiquido)
    {
        return new Produto(
            Guid.NewGuid(),
            "Produto de teste",
            quantidadeEstoque,
            precoLiquido);
    }

    [Fact]
    public void CarrinhoVazio_DevePossuirTotaisZerados()
    {
        var carrinho = new Carrinho();

        Assert.Empty(carrinho.Itens);
        Assert.Equal(0m, carrinho.Subtotal);
        Assert.Equal(0m, carrinho.Desconto);
        Assert.Equal(0m, carrinho.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AdicionarProdutoComQuantidadeInvalida_DeveLancarExcecao(
        int quantidade)
    {
        var produto = CriarProduto(10, 25m);
        var carrinho = new Carrinho();

        var excecao = Assert.Throws<RegraDeNegocioException>(
            () => carrinho.AdicionarProduto(produto, quantidade));

        Assert.Contains(
            "maior que zero",
            excecao.Message,
            StringComparison.OrdinalIgnoreCase);

        Assert.Empty(carrinho.Itens);
    }

    [Fact]
    public void SomarQuantidadeAcimaDoEstoque_DevePreservarQuantidadeAnterior()
    {
        var produto = CriarProduto(3, 20m);
        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);

        var excecao = Assert.Throws<RegraDeNegocioException>(
            () => carrinho.AdicionarProduto(produto, 2));

        Assert.Contains(
            "Estoque insuficiente",
            excecao.Message);

        var item = Assert.Single(carrinho.Itens);

        Assert.Equal(2, item.Quantidade);
        Assert.Equal(40m, carrinho.Subtotal);
    }

    [Fact]
    public void RemoverCupom_DeveRemoverDescontoERecalcularTotal()
    {
        var produto = CriarProduto(10, 100m);

        var cupom = new Cupom(
            Guid.NewGuid(),
            "10OFF",
            10m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AplicarCupom(cupom);
        carrinho.RemoverCupom();

        Assert.Null(carrinho.CupomAplicado);
        Assert.Null(carrinho.CupomId);
        Assert.Equal(200m, carrinho.Subtotal);
        Assert.Equal(0m, carrinho.Desconto);
        Assert.Equal(200m, carrinho.Total);
    }

    [Fact]
    public void CarrinhoFinalizado_DeveBloquearTodasAsAlteracoes()
    {
        var produto = CriarProduto(10, 100m);

        var cupom = new Cupom(
            Guid.NewGuid(),
            "10OFF",
            10m);

        var outroCupom = new Cupom(
            Guid.NewGuid(),
            "15OFF",
            15m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AplicarCupom(cupom);
        carrinho.Finalizar();

        Assert.Throws<RegraDeNegocioException>(
            () => carrinho.AlterarQuantidade(produto, 3));

        Assert.Throws<RegraDeNegocioException>(
            () => carrinho.RemoverProduto(produto.Id));

        Assert.Throws<RegraDeNegocioException>(
            () => carrinho.AplicarCupom(outroCupom));

        Assert.Throws<RegraDeNegocioException>(
            () => carrinho.RemoverCupom());

        Assert.Throws<RegraDeNegocioException>(
            () => carrinho.Finalizar());
    }
}