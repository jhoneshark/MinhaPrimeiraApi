using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Infra.Context;
using MinhaPrimeiraApi.Domain.DTOs.Mappings;
using MinhaPrimeiraApi.Extensions;
using MinhaPrimeiraApi.Filters;
using MinhaPrimeiraApi.Logging;
using MinhaPrimeiraApi.Middlewares;
using MinhaPrimeiraApi.Services;
using MinhaPrimeiraApi.Services.Schedules;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Remova o 'options => { options.Filters.Add(typeof(ApiExceptionFilter)); }' para funcionar o ApiExecptionMiddlewareExtensions
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
string mySqlConnection = builder.Configuration.GetValue<string>("DEFAULT_CONNECTION");

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddCatalogServices(builder.Configuration);

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information,
}));

builder.Services.AddAutoMapper(cfg => {}, typeof(ProductDTOMappingProfile).Assembly);

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExeptionHandler();
}

app.UseHangfireDashboardCustom();

app.RegisterHangfireJobs();

app.UseHttpsRedirection();

app.UseMiddleware<ApiLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();