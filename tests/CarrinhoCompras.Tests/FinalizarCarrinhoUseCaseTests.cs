using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.UseCases.Carrinhos;
using CarrinhoCompras.Domain.Entities;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Tests;

public sealed class FinalizarCarrinhoUseCaseTests
{
    [Fact]
    public async Task FinalizarCarrinho_DeveAlterarStatusEPreservarTotais()
    {
        var produto =
            new Produto(
                Guid.NewGuid(),
                "Produto de teste",
                quantidadeEstoque: 10,
                precoLiquido: 100m);

        var cupom =
            new Cupom(
                Guid.NewGuid(),
                "10OFF",
                percentualDesconto: 10m);

        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);
        carrinho.AplicarCupom(cupom);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new FinalizarCarrinhoUseCase(
                carrinhoRepository,
                produtoRepository);

        var resultado =
            await useCase.ExecutarAsync(carrinho.Id);

        Assert.Equal("Finalizado", resultado.Status);
        Assert.Equal(200m, resultado.Subtotal);
        Assert.Equal(20m, resultado.Desconto);
        Assert.Equal(180m, resultado.Total);

        Assert.NotNull(resultado.CupomAplicado);
        Assert.Equal(
            "10OFF",
            resultado.CupomAplicado.CodigoCupom);

        Assert.Equal(
            1,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task FinalizarCarrinhoInexistente_DeveLancarExcecao()
    {
        var carrinhoRepository =
            new CarrinhoRepositoryFake();

        var produtoRepository =
            new ProdutoRepositoryFake();

        var useCase =
            new FinalizarCarrinhoUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(Guid.NewGuid()));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task FinalizarCarrinhoJaFinalizado_NaoDeveSalvar()
    {
        var carrinho = new Carrinho();

        carrinho.Finalizar();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake();

        var useCase =
            new FinalizarCarrinhoUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RegraDeNegocioException>(
            () => useCase.ExecutarAsync(carrinho.Id));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
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