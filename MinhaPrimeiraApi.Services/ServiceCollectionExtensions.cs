using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Repository;

namespace MinhaPrimeiraApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddApplicationServices(services);
        AddAuthenticationServices(services, configuration); 
        return services;
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoriesRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JWT:SecretKey"];

        services.AddAuthorization();
        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidIssuer = configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });
    }
}