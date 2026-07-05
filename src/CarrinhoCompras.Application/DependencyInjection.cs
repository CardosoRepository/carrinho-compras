using CarrinhoCompras.Application.UseCases.Carrinhos;
using CarrinhoCompras.Application.UseCases.Produtos;

using Microsoft.Extensions.DependencyInjection;

namespace CarrinhoCompras.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ListarProdutosUseCase>();
        services.AddScoped<CriarCarrinhoUseCase>();
        services.AddScoped<ObterCarrinhoUseCase>();
        services.AddScoped<AdicionarItemUseCase>();
        services.AddScoped<AlterarQuantidadeItemUseCase>();
        services.AddScoped<RemoverItemUseCase>();

        return services;
    }
}