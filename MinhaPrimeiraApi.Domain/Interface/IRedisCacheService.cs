namespace MinhaPrimeiraApi.Domain.Interface;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetCacheAsync(string key, object data);
    Task InvalidateCacheAfterChangeAsync(string key);
}