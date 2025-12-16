using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Repository;
using MinhaPrimeiraApi.Services.Services;

namespace MinhaPrimeiraApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddApplicationServices(services);
        AddAuthenticationServices(services, configuration); 
        AddAuthenticationServices(services);
        AddSwaggerServices(services);
        AddPolicyCors(services);
        AddPolicysAuthorization(services);
        return services;
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoriesRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddPolicyCors(IServiceCollection services)
    {
        services.AddCors(options => 
        {
            // ---------------------------------------------------------
            // 1. POLÍTICA PADRÃO (Default)
            // ---------------------------------------------------------
            // Esta será usada se nenhuma outra for especificada no Controller.
            options.AddDefaultPolicy(builder => 
            {
                builder.AllowAnyOrigin()   // Permite qualquer IP/Domínio
                    .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE, OPTIONS
                    .AllowAnyHeader();  // Permite Authorization, X-Requested-With, etc
            });

            // ---------------------------------------------------------
            // 2. POLÍTICA NOMEADA: "ParceiroVip"
            // ---------------------------------------------------------
            // Cenário: Um parceiro que precisa acessar endpoints restritos.
            options.AddPolicy("ParceiroVip", builder => 
            {
                builder.WithOrigins("https://api.parceiro-vip.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            // ---------------------------------------------------------
            // 3. POLÍTICA NOMEADA: "ApenasLeituraPublica"
            // ---------------------------------------------------------
            // Cenário: Dados públicos (notícias, clima) que qualquer site pode consumir.
            options.AddPolicy("ApenasLeituraPublica", builder => 
            {
                builder.AllowAnyOrigin() // CUIDADO: Totalmente aberto
                    .WithMethods("GET", "OPTIONS") // Só permite ler
                    .AllowAnyHeader();
            });
        });
    }

    private static void AddPolicysAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RootOnly", policy => policy.RequireRole("Root"));
            //exemplo com duas condicoes 
            //options.AddPolicy("RootOnly", policy => policy.RequireRole("Root").RequireClaim("Id", "3"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

            options.AddPolicy("ExclusivePolicyOnly", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.IsInRole("Root")
                    
                    // mais exemplo abaixo
                    // context.User.HasClaim(claim => 
                    //     claim.Type == "Id" ||
                    //     context.User.IsInRole("Admin") || 
                    //     context.User.IsInRole("Root") || 
                    //     context.User.IsInRole("User"))
                    ));

        });
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

    // NEW AddAuthenticationServices
    private static void AddAuthenticationServices(IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }

    private static void AddSwaggerServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swaggerOptions =>
        {
            swaggerOptions.CustomSchemaIds(type => type.FullName);
            swaggerOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Input token like: Bearer {your token}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        }
                    },
                    new string[] { }
                }
            });
        });
    }
}