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
        // Está registrando a injeção de dependência, dizendo ao ASP.NET Core para usar CategoriesRepository sempre que alguém pedir por ICategoryRepository
        // services.AddScoped<ICategoryRepository, CategoriesRepository>();
        // services.AddScoped<IProductsRepository, ProductsRepository>();
        // services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}