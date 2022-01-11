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

    private async Task<TAppAuthorization> FindUniqueAsync(TAppAuthorization match)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await GetAppAuthorizationStore().FindUniqueAsync(match.AppId, match.UserId);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async Task<TAppAuthorization> GetAppAuthorizationAsync(long appId, TUserKey userId)
    {
        return await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);
    }

    public async Task<ManagerResult> AuthorizeAsync(long appId, TUserKey userId)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        TAppAuthorization existingAppAuthorization = await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        List<IScope> appScopes = GetAppAuthorizationStore().GetQueryableAppScopes(appId).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        if (existingAppAuthorization == null)
        {
            TAppAuthorization appAuthorization = new TAppAuthorization();
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
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    IScope scope = appScopes.SingleOrDefault(s => s.Name == unauthorizedScopeName);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

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
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        TAppAuthorization appAuthorization = await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        if (appAuthorization == null)
            return new ManagerResult(ApiMessages.AppNotAuthorized);

        return await DeleteAsync(appAuthorization.Id);
    }

    public List<string> GetScopes(long appAuthorizationId)
    {
        return GetAppAuthorizationStore().GetQueryableAppAuthorizationScopes(appAuthorizationId).OrderBy(s => s.Name).Select(s => s.Name).ToList();
    }

    public bool HasScope(long appAuthorizationId, string scopeName)
    {
        return GetAppAuthorizationStore().GetQueryableAppAuthorizationScopes(appAuthorizationId).Any(s => s.Name == scopeName);
    }

    public async Task<List<string>> GetUnauthorizedScopesAsync(long appId, TUserKey userId)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        TAppAuthorization appAuthorization = await GetAppAuthorizationStore().FindUniqueAsync(appId, userId);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        List<string> appScopes = GetAppAuthorizationStore().GetQueryableAppScopes(appId).Select(s => s.Name).ToList();

        if (appAuthorization == null)
            return appScopes;

        List<string> appAuthorizationScopes = GetAppAuthorizationStore().GetQueryableAppAuthorizationScopes(appAuthorization.Id)
            .Select(s => s.Name).ToList();

        List<string> unauthorizedScopes = new List<string>();

        appScopes.ForEach(appScope =>
        {
            if (!appAuthorizationScopes.Contains(appScope))
                unauthorizedScopes.Add(appScope);
        });

        return unauthorizedScopes;
    }

}
