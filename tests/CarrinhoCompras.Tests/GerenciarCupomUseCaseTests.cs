using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Application.DTOs.Carrinhos;
using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Application.UseCases.Carrinhos;
using CarrinhoCompras.Domain.Entities;
using CarrinhoCompras.Domain.Exceptions;

namespace CarrinhoCompras.Tests;

public sealed class GerenciarCupomUseCaseTests
{
    [Fact]
    public async Task AplicarCupom_DeveCalcularDescontoETotal()
    {
        var produto = CriarProduto(100m);
        var cupom = CriarCupom("10OFF", 10m);
        var carrinho = CriarCarrinhoComProduto(produto);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var cupomRepository =
            new CupomRepositoryFake([cupom]);

        var useCase =
            new AplicarCupomUseCase(
                carrinhoRepository,
                produtoRepository,
                cupomRepository);

        var resultado =
            await useCase.ExecutarAsync(
                carrinho.Id,
                new AplicarCupomRequest
                {
                    CodigoCupom = "10off"
                });

        Assert.NotNull(resultado.CupomAplicado);
        Assert.Equal(
            "10OFF",
            resultado.CupomAplicado.CodigoCupom);

        Assert.Equal(200m, resultado.Subtotal);
        Assert.Equal(20m, resultado.Desconto);
        Assert.Equal(180m, resultado.Total);

        Assert.Equal(
            1,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task AplicarOutroCupom_DeveSubstituirCupomAtual()
    {
        var produto = CriarProduto(100m);
        var cupom10 = CriarCupom("10OFF", 10m);
        var cupom15 = CriarCupom("15OFF", 15m);
        var carrinho = CriarCarrinhoComProduto(produto);

        carrinho.AplicarCupom(cupom10);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var cupomRepository =
            new CupomRepositoryFake([cupom10, cupom15]);

        var useCase =
            new AplicarCupomUseCase(
                carrinhoRepository,
                produtoRepository,
                cupomRepository);

        var resultado =
            await useCase.ExecutarAsync(
                carrinho.Id,
                new AplicarCupomRequest
                {
                    CodigoCupom = "15OFF"
                });

        Assert.NotNull(resultado.CupomAplicado);
        Assert.Equal(
            cupom15.Id,
            resultado.CupomAplicado.Id);

        Assert.Equal(30m, resultado.Desconto);
        Assert.Equal(170m, resultado.Total);
    }

    [Fact]
    public async Task AplicarCupomInvalido_NaoDeveSalvar()
    {
        var produto = CriarProduto(100m);
        var carrinho = CriarCarrinhoComProduto(produto);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var cupomRepository =
            new CupomRepositoryFake();

        var useCase =
            new AplicarCupomUseCase(
                carrinhoRepository,
                produtoRepository,
                cupomRepository);

        var excecao =
            await Assert.ThrowsAsync<RegraDeNegocioException>(
                () => useCase.ExecutarAsync(
                    carrinho.Id,
                    new AplicarCupomRequest
                    {
                        CodigoCupom = "INVALIDO"
                    }));

        Assert.Equal(
            "O cupom informado é inválido.",
            excecao.Message);

        Assert.Null(carrinho.CupomAplicado);

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task AplicarCupomComCodigoVazio_NaoDeveSalvar()
    {
        var carrinhoRepository =
            new CarrinhoRepositoryFake();

        var produtoRepository =
            new ProdutoRepositoryFake();

        var cupomRepository =
            new CupomRepositoryFake();

        var useCase =
            new AplicarCupomUseCase(
                carrinhoRepository,
                produtoRepository,
                cupomRepository);

        await Assert.ThrowsAsync<RegraDeNegocioException>(
            () => useCase.ExecutarAsync(
                Guid.NewGuid(),
                new AplicarCupomRequest
                {
                    CodigoCupom = " "
                }));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task AplicarCupomEmCarrinhoInexistente_DeveLancarExcecao()
    {
        var cupom = CriarCupom("10OFF", 10m);

        var carrinhoRepository =
            new CarrinhoRepositoryFake();

        var produtoRepository =
            new ProdutoRepositoryFake();

        var cupomRepository =
            new CupomRepositoryFake([cupom]);

        var useCase =
            new AplicarCupomUseCase(
                carrinhoRepository,
                produtoRepository,
                cupomRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(
                Guid.NewGuid(),
                new AplicarCupomRequest
                {
                    CodigoCupom = "10OFF"
                }));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverCupom_DeveRemoverDesconto()
    {
        var produto = CriarProduto(100m);
        var cupom = CriarCupom("10OFF", 10m);
        var carrinho = CriarCarrinhoComProduto(produto);

        carrinho.AplicarCupom(cupom);

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new RemoverCupomUseCase(
                carrinhoRepository,
                produtoRepository);

        var resultado =
            await useCase.ExecutarAsync(carrinho.Id);

        Assert.Null(resultado.CupomAplicado);
        Assert.Equal(200m, resultado.Subtotal);
        Assert.Equal(0m, resultado.Desconto);
        Assert.Equal(200m, resultado.Total);

        Assert.Equal(
            1,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverCupomDeCarrinhoFinalizado_NaoDeveSalvar()
    {
        var produto = CriarProduto(100m);
        var cupom = CriarCupom("10OFF", 10m);
        var carrinho = CriarCarrinhoComProduto(produto);

        carrinho.AplicarCupom(cupom);
        carrinho.Finalizar();

        var carrinhoRepository =
            new CarrinhoRepositoryFake([carrinho]);

        var produtoRepository =
            new ProdutoRepositoryFake([produto]);

        var useCase =
            new RemoverCupomUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RegraDeNegocioException>(
            () => useCase.ExecutarAsync(carrinho.Id));

        Assert.NotNull(carrinho.CupomAplicado);

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    [Fact]
    public async Task RemoverCupomDeCarrinhoInexistente_DeveLancarExcecao()
    {
        var carrinhoRepository =
            new CarrinhoRepositoryFake();

        var produtoRepository =
            new ProdutoRepositoryFake();

        var useCase =
            new RemoverCupomUseCase(
                carrinhoRepository,
                produtoRepository);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(
            () => useCase.ExecutarAsync(Guid.NewGuid()));

        Assert.Equal(
            0,
            carrinhoRepository.SalvarAlteracoesChamadas);
    }

    private static Produto CriarProduto(decimal precoLiquido)
    {
        return new Produto(
            Guid.NewGuid(),
            "Produto de teste",
            quantidadeEstoque: 10,
            precoLiquido);
    }

    private static Cupom CriarCupom(
        string codigo,
        decimal percentual)
    {
        return new Cupom(
            Guid.NewGuid(),
            codigo,
            percentual);
    }

    private static Carrinho CriarCarrinhoComProduto(
        Produto produto)
    {
        var carrinho = new Carrinho();

        carrinho.AdicionarProduto(produto, 2);

        return carrinho;
    }

    private sealed class CupomRepositoryFake
        : ICupomRepository
    {
        private readonly IReadOnlyCollection<Cupom> _cupons;

        public CupomRepositoryFake(
            IReadOnlyCollection<Cupom>? cupons = null)
        {
            _cupons =
                cupons ?? Array.Empty<Cupom>();
        }

        public Task<Cupom?> ObterPorCodigoAsync(
            string codigoCupom,
            CancellationToken cancellationToken = default)
        {
            var codigoNormalizado =
                codigoCupom
                    .Trim()
                    .ToUpperInvariant();

            var cupom =
                _cupons.SingleOrDefault(
                    item =>
                        item.CodigoCupom == codigoNormalizado);

            return Task.FromResult(cupom);
        }
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