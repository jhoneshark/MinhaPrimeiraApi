using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.Extensions;
using MinhaPrimeiraApi.Filters;
using MinhaPrimeiraApi.Logging;
using MinhaPrimeiraApi.Repository;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Está registrando a injeção de dependência, dizendo ao ASP.NET Core para usar CategoriesRepository sempre que alguém pedir por ICategoryRepository
builder.Services.AddScoped<ICategoryRepository, CategoriesRepository>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information,
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExeptionHandler();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();