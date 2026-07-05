using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.UseCases.Carrinhos;
using CarrinhoCompras.Domain.Entities;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Tests;

public sealed class AdicionarItemUseCaseTests
{
    [Fact]
    public async Task AdicionarItem_DevePersistirERetornarTotais()
    {
        var produto = CriarProduto(
            quantidadeEstoque: 10,
            precoLiquido: 100m);

        var carrinho = new Carrinho();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new AdicionarItemUseCase(
                carrinhoRepository,
                produtoRepository);

        var resultado =
            await useCase.ExecutarAsync(
                carrinho.Id,
                new AdicionarItemRequest
                {
                    ProdutoId = produto.Id,
                    Quantidade = 2
                });

        var item = Assert.Single(resultado.Itens);

        Assert.Equal(produto.Id, item.ProdutoId);
        Assert.Equal(2, item.Quantidade);
        Assert.Equal(100m, item.PrecoLiquidoUnitario);
        Assert.Equal(200m, item.PrecoTotal);
        Assert.Equal(200m, resultado.Subtotal);
        Assert.Equal(200m, resultado.Total);
        Assert.Equal(1, carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task AdicionarProdutoExistente_DeveSomarQuantidade()
    {
        var produto = CriarProduto(
            quantidadeEstoque: 10,
            precoLiquido: 50m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 1);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new AdicionarItemUseCase(
                carrinhoRepository,
                produtoRepository);

        var resultado =
            await useCase.ExecutarAsync(
                carrinho.Id,
                new AdicionarItemRequest
                {
                    ProdutoId = produto.Id,
                    Quantidade = 2
                });

        var item = Assert.Single(resultado.Itens);

        Assert.Equal(3, item.Quantidade);
        Assert.Equal(150m, item.PrecoTotal);
    }

    [Fact]
    public async Task AdicionarItemEmCarrinhoInexistente_DeveLancarExcecao()
    {
        var produto = CriarProduto(
            quantidadeEstoque: 10,
            precoLiquido: 100m);

        var carrinhoRepository =
            new CarrinhoRepositoryFake();

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new AdicionarItemUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(
                Guid.NewGuid(),
                new AdicionarItemRequest
                {
                    ProdutoId = produto.Id,
                    Quantidade = 1
                }));
    }

    [Fact]
    public async Task AdicionarProdutoInexistente_DeveLancarExcecao()
    {
        var carrinho = new Carrinho();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake();

        var useCase =
            new AdicionarItemUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(
                carrinho.Id,
                new AdicionarItemRequest
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 1
                }));
    }

    [Fact]
    public async Task AdicionarQuantidadeAcimaDoEstoque_NaoDeveSalvarAlteracoes()
    {
        var produto = CriarProduto(
            quantidadeEstoque: 2,
            precoLiquido: 100m);

        var carrinho = new Carrinho();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new AdicionarItemUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RegraDeNegocioException>(
            () => useCase.ExecutarAsync(
                carrinho.Id,
                new AdicionarItemRequest
                {
                    ProdutoId = produto.Id,
                    Quantidade = 3
                }));

        Assert.Empty(carrinho.Itens);
        Assert.Equal(0, carrinhoRepository.SalvarAlteracoesChamadas);
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

    private sealed class ProdutoRepositoryFake
        : IProdutoRepository
    {
        private readonly IReadOnlyCollection<Produto> _produtos;

        public ProdutoRepositoryFake(
            IReadOnlyCollection<Produto>? produtos = null)
        {
            _produtos =
                produtos ?? Array.Empty<Produto>();
        }

        public Task<IReadOnlyCollection<Produto>> ListarAsync(
            CancellationToken cancellationToken = default)
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

    private sealed class CarrinhoRepositoryFake
        : ICarrinhoRepository
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

        public int SalvarAlteracoesChamadas { get; private set; }

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

        public Task SalvarAlteracoesAsync(
            CancellationToken cancellationToken = default)
        {
            SalvarAlteracoesChamadas++;

            return Task.CompletedTask;
        }
    }
}