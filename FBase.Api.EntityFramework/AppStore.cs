using FBase.Api;
using FBase.DotNetCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.Api.EntityFramework
{
    public class AppStore<TUser, TUserKey> : EntityStoreBase<App<TUser, TUserKey>, long>, IAppStore<App<TUser, TUserKey>, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public AppStore(DbContext context) : base(context)
        {
        }

        public async Task AddScopeToAppAsync(int scopeId, long appId)
        {
            ScopeApp<TUser, TUserKey> scopeApp = new ScopeApp<TUser, TUserKey>
            {
                ScopeId = scopeId,
                AppId = appId
            };

            db.Set<ScopeApp<TUser, TUserKey>>().Add(scopeApp);
            await db.SaveChangesAsync();
        }

        public async Task<App<TUser, TUserKey>> FindByNameAsync(string name)
        {
            return await db.Set<App<TUser, TUserKey>>().SingleOrDefaultAsync(a => a.Name == name);
        }

        public IQueryable<App<TUser, TUserKey>> GetQueryableApps(TUserKey userId)
        {
            return db.Set<App<TUser, TUserKey>>().Where(a => a.UserId.Equals(userId)).AsQueryable();
        }

        public IQueryable<IScope> GetQueryableScopes(long? appId = null)
        {
            return (appId.HasValue)
                ? db.Set<ScopeApp<TUser, TUserKey>>().Include(sa => sa.Scope).Where(sa => sa.AppId == appId).Select(sa => sa.Scope)
                : db.Set<Scope>().AsQueryable();
        }

        public async Task RemoveScopeFromAppAsync(int scopeId, long appId)
        {
            ScopeApp<TUser, TUserKey> scopeApp = await db.Set<ScopeApp<TUser, TUserKey>>().SingleOrDefaultAsync(sa => sa.ScopeId == scopeId && sa.AppId == appId);

            if (scopeApp != null)
            {
                db.Set<ScopeApp<TUser, TUserKey>>().Remove(scopeApp);
                await db.SaveChangesAsync();
            }
        }
    }
}
