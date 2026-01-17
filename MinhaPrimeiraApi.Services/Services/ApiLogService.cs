using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Infra.Context;

namespace MinhaPrimeiraApi.Services.Services;

public class ApiLogService : IApiLogService
{
    private readonly AppDbContext _dbContext;

    public ApiLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveLogAsync(ApiResponseLog log)
    {
        _dbContext.ApiResponseLog.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}