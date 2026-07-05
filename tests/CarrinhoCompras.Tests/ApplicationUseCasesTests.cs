using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.UseCases.Carrinhos;
using CarrinhoCompras.Application.UseCases.Produtos;
using CarrinhoCompras.Domain.Entities;

namespace CarrinhoCompras.Tests;

public sealed class ApplicationUseCasesTests
{
    [Fact]
    public async Task ListarProdutos_DeveRetornarProdutosMapeados()
    {
        var produto = CriarProduto(
            "Notebook",
            quantidadeEstoque: 10,
            precoLiquido: 3500m);

        var produtoRepository = new ProdutoRepositoryFake([produto]);

        var useCase = new ListarProdutosUseCase(produtoRepository);

        var resultado = await useCase.ExecutarAsync();

        var produtoResponse = Assert.Single(resultado);

        Assert.Equal(produto.Id, produtoResponse.Id);
        Assert.Equal("Notebook", produtoResponse.DescricaoProduto);
        Assert.Equal(10, produtoResponse.QuantidadeEstoque);
        Assert.Equal(3500m, produtoResponse.PrecoLiquido);
    }

    [Fact]
    public async Task CriarCarrinho_DevePersistirCarrinhoAberto()
    {
        var carrinhoRepository = new CarrinhoRepositoryFake();

        var useCase = new CriarCarrinhoUseCase(carrinhoRepository);

        var resultado = await useCase.ExecutarAsync();

        Assert.Single(carrinhoRepository.Carrinhos);
        Assert.Equal("Aberto", resultado.Status);
        Assert.Empty(resultado.Itens);
        Assert.Equal(0m, resultado.Subtotal);
        Assert.Equal(0m, resultado.Desconto);
        Assert.Equal(0m, resultado.Total);
    }

    [Fact]
    public async Task ObterCarrinho_DeveRetornarDadosDoProduto()
    {
        var produto = CriarProduto(
            "Monitor",
            quantidadeEstoque: 8,
            precoLiquido: 1200m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);

        var carrinhoRepository = new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository = new ProdutoRepositoryFake([produto]);

        var useCase = new ObterCarrinhoUseCase(carrinhoRepository, produtoRepository);

        var resultado = await useCase.ExecutarAsync(carrinho.Id);

        var item = Assert.Single(resultado.Itens);

        Assert.Equal(produto.Id, item.ProdutoId);
        Assert.Equal("Monitor", item.DescricaoProduto);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(1200m, item.PrecoLiquidoUnitario);
        Assert.Equal(2400m, item.PrecoTotal);
        Assert.Equal(8, item.QuantidadeDisponivelEstoque);
    }

    [Fact]
    public async Task ObterCarrinhoInexistente_DeveLancarExcecao()
    {
        var carrinhoRepository = new CarrinhoRepositoryFake();

        var produtoRepository = new ProdutoRepositoryFake();

        var useCase = new ObterCarrinhoUseCase(carrinhoRepository, produtoRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(Guid.NewGuid()));
    }

    private static Produto CriarProduto(
        string descricao,
        int quantidadeEstoque,
        decimal precoLiquido)
    {
        return new Produto(
            Guid.NewGuid(),
            descricao,
            quantidadeEstoque,
            precoLiquido);
    }

    private sealed class ProdutoRepositoryFake : IProdutoRepository
    {
        private readonly IReadOnlyCollection<Produto> _produtos;

        public ProdutoRepositoryFake(IReadOnlyCollection<Produto>? produtos = null)
        {
            _produtos = produtos ?? Array.Empty<Produto>();
        }

        public Task<IReadOnlyCollection<Produto>> ListarAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_produtos);
        }

        public Task<Produto?> ObterPorIdAsync(
            Guid produtoId,
            CancellationToken cancellationToken = default)
        {
            var produto =
                _produtos.SingleOrDefault(
                    item => item.Id == produtoId);

            return Task.FromResult(produto);
        }

        public Task<IReadOnlyCollection<Produto>> ObterPorIdsAsync(
            IEnumerable<Guid> produtoIds,
            CancellationToken cancellationToken = default)
        {
            var ids = produtoIds.ToHashSet();

            IReadOnlyCollection<Produto> produtos =
                _produtos
                    .Where(produto => ids.Contains(produto.Id))
                    .ToArray();

            return Task.FromResult(produtos);
        }
    }

    private sealed class CarrinhoRepositoryFake : ICarrinhoRepository
    {
        private readonly Dictionary<Guid, Carrinho> _carrinhos;

        public CarrinhoRepositoryFake(
            IReadOnlyCollection<Carrinho>? carrinhos = null)
        {
            _carrinhos =
                carrinhos?.ToDictionary(
                    carrinho => carrinho.Id)
                ?? [];
        }

        public IReadOnlyCollection<Carrinho> Carrinhos => _carrinhos.Values.ToArray();

        public Task<Carrinho?> ObterPorIdAsync(
            Guid carrinhoId,
            CancellationToken cancellationToken = default)
        {
            _carrinhos.TryGetValue(
                carrinhoId,
                out var carrinho);

            return Task.FromResult(carrinho);
        }

        public Task<Carrinho?> ObterParaAtualizacaoAsync(
            Guid carrinhoId,
            CancellationToken cancellationToken = default)
        {
            return ObterPorIdAsync(
                carrinhoId,
                cancellationToken);
        }

        public Task AdicionarAsync(
            Carrinho carrinho,
            CancellationToken cancellationToken = default)
        {
            _carrinhos[carrinho.Id] = carrinho;

            return Task.CompletedTask;
        }

        public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}