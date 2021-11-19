using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBase.ApiServer.EntityFramework
{
    public class AppAuthorizationStore<TUser, TUserKey> : EntityStoreBase<AppAuthorization<TUser, TUserKey>, long>, IAppAuthorizationStore<AppAuthorization<TUser, TUserKey>, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public AppAuthorizationStore(DbContext context) : base(context)
        {
        }

        public async Task AddScopeToAppAuthorizationAsync(int scopeId, long appAuthorizationId)
        {
            ScopeAppAuthorization<TUser, TUserKey> scopeAppAuthorization = new ScopeAppAuthorization<TUser, TUserKey>
            {
                ScopeId = scopeId,
                AppAuthorizationId = appAuthorizationId
            };

            db.Set<ScopeAppAuthorization<TUser, TUserKey>>().Add(scopeAppAuthorization);
            await db.SaveChangesAsync();
        }

        public async Task<AppAuthorization<TUser, TUserKey>> FindUniqueAsync(long appId, TUserKey userId)
        {
            return await db.Set<AppAuthorization<TUser, TUserKey>>().SingleOrDefaultAsync(aa => aa.AppId == appId && aa.UserId.Equals(userId));
        }


        public IQueryable<AppAuthorization<TUser, TUserKey>> GetQueryableAppAuthorization()
        {
            return db.Set<AppAuthorization<TUser, TUserKey>>().AsQueryable();
        }

        public IQueryable<IScope> GetQueryableAppAuthorizationScopes(long? appAuthorizationId = null)
        {
            return (appAuthorizationId.HasValue)
              ? db.Set<ScopeAppAuthorization<TUser, TUserKey>>().Include(sa => sa.Scope).Where(sa => sa.AppAuthorizationId == appAuthorizationId).Select(sa => sa.Scope)
              : db.Set<Scope>().AsQueryable();
        }

        public IQueryable<IScope?> GetQueryableAppScopes(long? appId)
        {
            return (appId.HasValue)
               ? db.Set<ScopeApp<TUser, TUserKey>>().Include(sa => sa.Scope).Where(sa => sa.AppId == appId).Select(sa => sa.Scope)
               : db.Set<Scope>().AsQueryable();
        }
    }
}
