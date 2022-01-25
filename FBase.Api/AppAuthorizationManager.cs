using FBase.Foundations;

namespace FBase.Api;

public class AppAuthorizationManager<TAppAuthorization, TUserKey> : ManagerBase<TAppAuthorization, long>
    where TAppAuthorization : class, IAppAuthorization<TUserKey>, new()
    where TUserKey : IEquatable<TUserKey>
{
    public AppAuthorizationManager(IAppAuthorizationStore<TAppAuthorization, TUserKey> store) : base(store)
    {
    }

    private IAppAuthorizationStore<TAppAuthorization, TUserKey> GetAppAuthorizationStore()
    {
        return (IAppAuthorizationStore<TAppAuthorization, TUserKey>)Store;
    }

    private async Task<TAppAuthorization?> FindUniqueAsync(TAppAuthorization match)
    {
        return await GetAppAuthorizationStore().FindUniqueAsync(match.AppId, match.UserId);
    }

    public async Task<TAppAuthorization> GetAppAuthorizationAsync(long appId, TUserKey userId)
    {
        return await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);
    }

    public async Task<ManagerResult> AuthorizeAsync(long appId, TUserKey userId)
    {
        TAppAuthorization? existingAppAuthorization = await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);
        List<IScope?> appScopes = GetAppAuthorizationStore().GetQueryableAppScopes(appId)?.ToList() ?? new List<IScope?>();

        if (existingAppAuthorization == null)
        {
            TAppAuthorization appAuthorization = new();
            appAuthorization.AppId = appId;
            appAuthorization.UserId = userId;

            var createRes = await DataUtils.CreateAsync(
                entity: appAuthorization,
                store: GetAppAuthorizationStore(),
                findUniqueAsync: FindUniqueAsync);

            if (!createRes.Success)
                return createRes;

            // Add all of the scopes of the app to the scopes of this authorization
            appScopes.ForEach(appScope =>
            {
                if (appScope != null)
                    GetAppAuthorizationStore().AddScopeToAppAuthorizationAsync(appScope.Id, appAuthorization.Id);
            });
        }
        else
        {
            List<string> unauthorizedScopes = await GetUnauthorizedScopesAsync(appId, userId);

            if (unauthorizedScopes.Count > 0)
            {
                unauthorizedScopes.ForEach(unauthorizedScopeName =>
                {
                    IScope? scope = appScopes.SingleOrDefault(s => s?.Name == unauthorizedScopeName);

                    if (scope != null)
                        GetAppAuthorizationStore().AddScopeToAppAuthorizationAsync(scope.Id, existingAppAuthorization.Id);
                });
            }
        }

        return new ManagerResult();
    }

    public async Task<ManagerResult> DeleteAsync(long id)
    {
        return await DataUtils.DeleteAsync(
            id: id,
            store: GetAppAuthorizationStore());
    }

    public async Task<ManagerResult> RevokeAsync(long appId, TUserKey userId)
    {
        TAppAuthorization? appAuthorization = await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);

        if (appAuthorization == null)
            return new ManagerResult(ApiMessages.AppNotAuthorized);

        return await DeleteAsync(appAuthorization.Id);
    }

    public List<string> GetScopes(long appAuthorizationId)
    {
        return GetAppAuthorizationStore().GetQueryableAppAuthorizationScopes(appAuthorizationId)
            .OrderBy(s => (s != null) ? s.Name : "").Select(s => (s != null) ? s.Name : "").ToList();
    }

    public bool HasScope(long appAuthorizationId, string scopeName)
    {
        return GetAppAuthorizationStore().GetQueryableAppAuthorizationScopes(appAuthorizationId).Any(s => s != null && s.Name == scopeName);
    }

    public async Task<List<string>> GetUnauthorizedScopesAsync(long appId, TUserKey userId)
    {
        TAppAuthorization? appAuthorization = await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);

        List<string> appScopes = GetAppAuthorizationStore().GetQueryableAppScopes(appId).Select(s => (s != null) ? s.Name : "").ToList();

        if (appAuthorization == null)
            return appScopes;

        List<string> appAuthorizationScopes = GetAppAuthorizationStore().GetQueryableAppAuthorizationScopes(appAuthorization.Id)
            .Select(s => (s != null) ? s.Name : "").ToList();

        List<string> unauthorizedScopes = new();

        appScopes.ForEach(appScope =>
        {
            if (!appAuthorizationScopes.Contains(appScope))
                unauthorizedScopes.Add(appScope);
        });

        return unauthorizedScopes;
    }

}
