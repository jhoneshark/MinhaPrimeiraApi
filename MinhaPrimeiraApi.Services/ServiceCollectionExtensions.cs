using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Repository;
using MinhaPrimeiraApi.Infra.Interceptors;
using MinhaPrimeiraApi.Services.Services;

namespace MinhaPrimeiraApi.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddApplicationServices(services);
        AddAuthenticationServices(services, configuration);
        AddAuthenticationServices(services);
        AddRedisServices(services, configuration);
        AddHangfireServices(services, configuration);
        AddSwaggerServices(services);
        AddRateLimiter(services);
        AddPolicyCors(services);
        AddPolicysAuthorization(services);
        AddApiVersioningServices(services);
        return services;
    }

    private static void AddHangfireServices(IServiceCollection services, IConfiguration configuration)
    {
        // var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        // var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
        // var dbUser = Environment.GetEnvironmentVariable("DB_USERNAME");
        // var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        // var dbName = Environment.GetEnvironmentVariable("DB_DATABASE");
        //
        // string connectionString =
        //     $"Server={dbHost};Port={dbPort};User Id={dbUser};Password=\"{dbPassword}\";Database={dbName};Allow User Variables=true;";
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1 });

        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    connectionString,
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 25000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire",
                    }
                )
            ));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 5;
            options.Queues = new[] { "default", "critical" };
            options.ServerTimeout = TimeSpan.FromMinutes(5);
            options.ShutdownTimeout = TimeSpan.FromMinutes(15);
        });
    }

    private static void AddRedisServices(IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
        // var redisConnectionString = configuration.GetConnectionString("RedisCloud");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "MinhaPrimeiraApi_";
        });
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoriesRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IApiLogService, ApiLogService>();
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<TesteService>();
    }
    
    private static void AddApiVersioningServices(IServiceCollection services)
    {
        // o AddApiExplorer é para o swagger
        // se nao explicitar com o ApiVersionReader um esquema de versionamento o padrao é url query string
        // exemplos
        // url http://localhost:5041/api/v1/version1
        // queryString http://localhost:5041/api/version2?api-version=2.0
        
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("api-version")
            );
        }).AddApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'V";
            opt.SubstituteApiVersionInUrl = true;
        });

    }
    
    private static void AddRateLimiter(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Configuração geral de rejeição
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 1. Política para área pública (Global, sem particionamento por IP neste exemplo simples)
            options.AddFixedWindowLimiter("Publico", opt =>
            {
                opt.PermitLimit = 1000;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueLimit = 0;
            });

            // 2. Política "API_Free" (Particionada por IP)
            options.AddPolicy("API_Free", httpContext =>
            {
                var userIdentifier = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                                     ?? httpContext.Connection.RemoteIpAddress?.ToString() 
                                     ?? "anonymous";
            
                return RateLimitPartition.GetFixedWindowLimiter(userIdentifier, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromSeconds(30),
                    QueueLimit = 0
                });
            });

            // 3. Política "API_Premium" (Particionada por IP)
            options.AddPolicy("API_Premium", httpContext =>
            {
                var userIdentifier = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(userIdentifier, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,         // 100 requisições (muito mais limite)
                    Window = TimeSpan.FromSeconds(10),
                    QueueLimit = 2
                });
            });
        });
    }

    private static void AddRateLimiterGlobal(IServiceCollection services)
    {
        services.AddRateLimiter(rateLimitOptions =>
        {
            rateLimitOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimitOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => 
            {
                // Tenta pegar o IP, se for nulo (localhost/teste), usa "unknown"
                var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: remoteIp, // MUDANÇA: Particiona por IP, não por Host
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 10, // MUDANÇA: Aumentado para 10 reqs
                        QueueLimit = 2,   // MUDANÇA: Permite enfileirar 2 reqs breves
                        Window = TimeSpan.FromSeconds(5)
                    });
            });
        });
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