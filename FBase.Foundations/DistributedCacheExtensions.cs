using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FBase.Foundations;
public static class DistributedCacheExtensions
{
  /// <summary>
  /// Retrieves a value from the distributed cache if it exists;
  /// otherwise invokes the supplied async getter, stores the result,
  /// and returns it.
  /// </summary>
  public static async Task<T> GetOrSetAsync<T>(
      this IDistributedCache cache,
      string key,
      Func<Task<T>> getterIfNotRegistered,
      DistributedCacheEntryOptions? options = null,
      JsonSerializerOptions? jsonOptions = null)
  {
    var cachedJson = await cache.GetStringAsync(key);
    if (!string.IsNullOrEmpty(cachedJson))
    {
      return JsonSerializer.Deserialize<T>(cachedJson, jsonOptions)!;
    }

    var value = await getterIfNotRegistered();

    var jsonToCache = JsonSerializer.Serialize(value, jsonOptions);
    await cache.SetStringAsync(key, jsonToCache, options ?? new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    });

    return value;
  }

  /// <summary>
  /// Retrieves a value from the distributed cache if it exists;
  /// otherwise invokes the supplied synchronous getter, stores the result,
  /// and returns it.
  /// </summary>
  public static T GetOrSet<T>(
      this IDistributedCache cache,
      string key,
      Func<T> getterIfNotRegistered,
      DistributedCacheEntryOptions? options = null,
      JsonSerializerOptions? jsonOptions = null)
  {
    var cachedJson = cache.GetString(key);
    if (!string.IsNullOrEmpty(cachedJson))
    {
      return JsonSerializer.Deserialize<T>(cachedJson, jsonOptions)!;
    }

    var value = getterIfNotRegistered();

    var jsonToCache = JsonSerializer.Serialize(value, jsonOptions);
    cache.SetString(key, jsonToCache, options ?? new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    });

    return value;
  }
}
