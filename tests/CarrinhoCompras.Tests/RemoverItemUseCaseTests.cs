using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.UseCases.Carrinhos;
using CarrinhoCompras.Domain.Entities;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Tests;

public sealed class RemoverItemUseCaseTests
{
    [Fact]
    public async Task RemoverItem_DeveRemoverProdutoERecalcularTotais()
    {
        var produtoRemovido =
            CriarProduto(
                "Produto removido",
                quantidadeEstoque: 10,
                precoLiquido: 50m);

        var produtoMantido =
            CriarProduto(
                "Produto mantido",
                quantidadeEstoque: 10,
                precoLiquido: 100m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produtoRemovido, 2);
        carrinho.AdicionarProduto(produtoMantido, 1);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake(
                [produtoRemovido, produtoMantido]);

        var useCase =
            new RemoverItemUseCase(
                carrinhoRepository,
                produtoRepository);

        var resultado =
            await useCase.ExecutarAsync(
                carrinho.Id,
                produtoRemovido.Id);

        var item = Assert.Single(resultado.Itens);

        Assert.Equal(produtoMantido.Id, item.ProdutoId);
        Assert.Equal(1, item.Quantidade);
        Assert.Equal(100m, resultado.Subtotal);
        Assert.Equal(100m, resultado.Total);

        Assert.Single(carrinho.Itens);

        Assert.Equal(
            1,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverUltimoItem_DeveRetornarCarrinhoVazio()
    {
        var produto =
            CriarProduto(
                "Produto",
                quantidadeEstoque: 10,
                precoLiquido: 50m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new RemoverItemUseCase(
                carrinhoRepository,
                produtoRepository);

        var resultado =
            await useCase.ExecutarAsync(
                carrinho.Id,
                produto.Id);

        Assert.Empty(resultado.Itens);
        Assert.Equal(0m, resultado.Subtotal);
        Assert.Equal(0m, resultado.Desconto);
        Assert.Equal(0m, resultado.Total);
        Assert.Empty(carrinho.Itens);

        Assert.Equal(
            1,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverItemDeCarrinhoInexistente_DeveLancarExcecao()
    {
        var carrinhoRepository =
            new CarrinhoRepositoryFake();

        var produtoRepository =
            new ProdutoRepositoryFake();

        var useCase =
            new RemoverItemUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(
                Guid.NewGuid(),
                Guid.NewGuid()));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverProdutoAusente_NaoDeveSalvarAlteracoes()
    {
        var carrinho = new Carrinho();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake();

        var useCase =
            new RemoverItemUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RegraDeNegocioException>(
            () => useCase.ExecutarAsync(
                carrinho.Id,
                Guid.NewGuid()));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverItemDeCarrinhoFinalizado_NaoDeveSalvar()
    {
        var produto =
            CriarProduto(
                "Produto",
                quantidadeEstoque: 10,
                precoLiquido: 50m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.Finalizar();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new RemoverItemUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RegraDeNegocioException>(
            () => useCase.ExecutarAsync(
                carrinho.Id,
                produto.Id));

        Assert.Single(carrinho.Itens);

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
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