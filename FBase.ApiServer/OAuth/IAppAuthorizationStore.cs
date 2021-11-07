using FBase.Foundations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public interface IAppAuthorizationStore<TAppAuthorization, TUserKey> : IAsyncStore<TAppAuthorization, long>
        where TAppAuthorization : class, IAppAuthorization<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        Task<TAppAuthorization> FindUniqueAsync(long appId, TUserKey userId);
        IQueryable<TAppAuthorization> GetQueryableAppAuthorization();
        IQueryable<IScope> GetQueryableAppAuthorizationScopes(long? appAuthorizationId = null);
        IQueryable<IScope> GetQueryableAppScopes(long? appId);
        Task AddScopeToAppAuthorizationAsync(int scopeId, long appAuthorizationId);
    }
}
