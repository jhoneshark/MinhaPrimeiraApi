using Microsoft.Extensions.DependencyInjection;

namespace MinhaPrimeiraApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services)
    {
        AddApplicationServices(services);
        return services;
    }

    public static void AddApplicationServices(IServiceCollection services)
    {
    }
}