using FBase.Foundations;

namespace FBase.Api;

public interface IAppStore<TApp, TUserKey> : IAsyncStore<TApp, long>
    where TApp : class, IApp<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
#nullable enable
    Task<TApp?> FindByNameAsync(string name);
    IQueryable<TApp> GetQueryableApps(TUserKey userId);
    IQueryable<IScope> GetQueryableScopes(long? appId = null);
    Task AddScopeToAppAsync(int scopeId, long appId);
    Task RemoveScopeFromAppAsync(int scopeId, long appId);
}
