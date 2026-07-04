using System.Reflection;
using System.Text.Json;

using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarrinhoCompras.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task InitializeDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<CarrinhoComprasDbContext>();

        await context.Database.MigrateAsync(cancellationToken);

        var possuiAlteracoes = false;

        if (!await context.Produtos.AnyAsync(cancellationToken))
        {
            var produtos =
                LerArquivo<ProdutoSeedDto>("produtos.json")
                    .Select(produto =>
                        new Produto(
                            produto.Id,
                            produto.DescricaoProduto,
                            produto.QuantidadeEstoque,
                            produto.PrecoLiquido));

            context.Produtos.AddRange(produtos);

            possuiAlteracoes = true;
        }

        if (!await context.Cupons.AnyAsync(cancellationToken))
        {
            var cupons =
                LerArquivo<CupomSeedDto>("cupons.json")
                    .Select(cupom =>
                        new Cupom(
                            cupom.Id,
                            cupom.CodigoCupom,
                            cupom.PercentualDesconto));

            context.Cupons.AddRange(cupons);

            possuiAlteracoes = true;
        }

        if (possuiAlteracoes)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static IReadOnlyCollection<T> LerArquivo<T>(string nomeArquivo)
    {
        var assembly = typeof(DatabaseInitializer).Assembly;

        var nomeRecurso = $"{assembly.GetName().Name}.Persistence.SeedData.{nomeArquivo}";

        using var stream =
            assembly.GetManifestResourceStream(nomeRecurso)
            ?? throw new InvalidOperationException($"Não foi possível ler o arquivo de seed '{nomeArquivo}'.");

        return JsonSerializer.Deserialize<List<T>>(
            stream,
            JsonOptions)
            ?? throw new InvalidOperationException($"Não foi possível ler o arquivo de seed '{nomeArquivo}'.");
    }

    private sealed record ProdutoSeedDto(
        Guid Id,
        string DescricaoProduto,
        int QuantidadeEstoque,
        decimal PrecoLiquido);

    private sealed record CupomSeedDto(
        Guid Id,
        string CodigoCupom,
        decimal PercentualDesconto);
}