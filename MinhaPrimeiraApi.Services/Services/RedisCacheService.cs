using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MinhaPrimeiraApi.Domain.Interface;

namespace MinhaPrimeiraApi.Services.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;


    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(data))
            return default;
        
        try 
        {
            return JsonSerializer.Deserialize<T>(data);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetCacheAsync(string key, object data)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        var jsonData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(key, jsonData, options);
    }

    public async Task InvalidateCacheAfterChangeAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}