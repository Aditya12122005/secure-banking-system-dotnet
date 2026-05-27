using Banking.Application.Interfaces;

using Microsoft.Extensions.Caching.Distributed;

using System.Text.Json;

namespace Banking.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiry)
    {
        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiry
            };

        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            options);
    }

    public async Task<T?> GetAsync<T>(
        string key)
    {
        var json =
            await _cache.GetStringAsync(key);

        if (json == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveAsync(
        string key)
    {
        await _cache.RemoveAsync(key);
    }
}