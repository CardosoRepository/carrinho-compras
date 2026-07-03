using CarrinhoCompras.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CarrinhoCompras.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "A string de conexão com o banco é obrigatória.",
                nameof(connectionString));
        }

        services.AddDbContext<CarrinhoComprasDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}