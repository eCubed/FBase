using FBase.Foundations;

namespace FBase.Api;

public class AppManager<TApp, TUserKey> : ManagerBase<TApp, long>
    where TApp : class, IApp<TUserKey>, new()
    where TUserKey : IEquatable<TUserKey>
{
    public AppManager(IAppStore<TApp, TUserKey> store) : base(store)
    {
    }

    private IAppStore<TApp, TUserKey> GetAppStore()
    {
        return (IAppStore<TApp, TUserKey>)Store;
    }

    public async Task<TApp?> FindByNameAsync(string name)
    {
        return await GetAppStore().FindByNameAsync(name);
    }

    private async Task<TApp?> FindUniqueAsync(TApp match)
    {
        return await GetAppStore().FindByNameAsync(match.Name);
    }

    public List<TApp> GetApps(TUserKey userId)
    {
        return GetAppStore().GetQueryableApps(userId).OrderBy(a => a.Name).ToList();
    }

    public async Task<ManagerResult<TApp>> CreateAsync(string name, TUserKey userId)
    {
        TApp app = new TApp();
        app.Name = name;
        app.UserId = userId;

        var createRes = await DataUtils.CreateAsync(
            entity: app,
            store: GetAppStore(),
            findUniqueAsync: FindUniqueAsync);

        if (!createRes.Success)
            return new ManagerResult<TApp>(createRes.Errors);

        return new ManagerResult<TApp>(app);
    }

    public async Task<ManagerResult> UpdateAsync(long id, string name, TUserKey requestorId)
    {
        return await DataUtils.UpdateAsync(
            id: id,
            store: GetAppStore(),
            findUniqueAsync: FindUniqueAsync,
            canUpdate: (app) =>
            {
                if (!app.UserId.Equals(requestorId))
                    return new ManagerResult(ManagerErrors.Unauthorized);

                return new ManagerResult();
            },
            fillNewValues: (original) =>
            {
                original.Name = name;
            });
    }

    public async Task<ManagerResult> DeleteAsync(long id, TUserKey requestorId)
    {
        return await DataUtils.DeleteAsync(
            id: id,
            store: GetAppStore(),
            canDelete: (app) =>
            {
                if (!app.UserId.Equals(requestorId))
                    return new ManagerResult(ManagerErrors.Unauthorized);

                return new ManagerResult();
            });
    }

    public List<string> GetScopes(long appId)
    {
        return GetAppStore().GetQueryableScopes(appId)
            .OrderBy(s => (s != null) ? s.Name : "")
            .Select(s => (s != null) ? s.Name : "").ToList();
    }

    public bool HasScope(long appId, string scopeName)
    {
        return GetAppStore().GetQueryableScopes(appId).Any(s => s.Name == scopeName);
    }

    public async Task<ManagerResult> AddScopeToAppAsync(string scopeName, long appId)
    {
        TApp? app = await FindByIdAsync(appId);

        if (app == null)
            return new ManagerResult(ManagerErrors.RecordNotFound);
        
        IScope? scope = GetAppStore().GetQueryableScopes().SingleOrDefault(s => s.Name == scopeName);

        if (scope == null)
            return new ManagerResult(ApiMessages.ScopeNotFound);

        await GetAppStore().AddScopeToAppAsync(scope.Id, app.Id);

        return new ManagerResult();
    }

    public async Task<ManagerResult> RemoveScopeFromAppAsync(string scopeName, long appId)
    {
        TApp? app = await FindByIdAsync(appId);

        if (app == null)
            return new ManagerResult(ManagerErrors.RecordNotFound);

        IScope? scope = GetAppStore().GetQueryableScopes(appId).SingleOrDefault(s => (s != null) ? s.Name == scopeName : false);

        if (scope == null)
            return new ManagerResult(ApiMessages.ScopeNotInApp);

        await GetAppStore().RemoveScopeFromAppAsync(scope.Id, app.Id);

        return new ManagerResult();
    }
}
