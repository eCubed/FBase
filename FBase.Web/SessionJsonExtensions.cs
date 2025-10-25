using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace FBase.Web;
public static class SessionJsonExtensions
{
  private static void SetJson<T>(this ISession session, string key, T value, JsonSerializerOptions? options = null)
        => session.SetString(key, JsonSerializer.Serialize(value, options));

  private static T? GetJson<T>(this ISession session, string key, JsonSerializerOptions? options = null)
  {
    var json = session.GetString(key);
    return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json!, options);
  }

  /// <summary>
  /// Session-based equivalent of GetOrSetAsync (no per-item expiration; session IdleTimeout applies).
  /// </summary>
  public static async Task<T> GetOrSetAsync<T>(
      this ISession session,
      string key,
      Func<Task<T>> getterIfNotRegistered,
      JsonSerializerOptions? jsonOptions = null)
  {
    var existing = session.GetJson<T>(key, jsonOptions);
    if (existing is not null) return existing;

    var value = await getterIfNotRegistered();
    session.SetJson(key, value, jsonOptions);
    return value;
  }

  public static T GetOrSetAsync<T>(
      this ISession session,
      string key,
      Func<T> getterIfNotRegistered,
      JsonSerializerOptions? jsonOptions = null)
  {
    var existing = session.GetJson<T>(key, jsonOptions);
    if (existing is not null) return existing;

    var value = getterIfNotRegistered();
    session.SetJson(key, value, jsonOptions);
    return value;
  }
}
