using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class AppStore : EntityStoreBase<App, long>, IAppStore<App, int>
    {
        public AppStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public async Task AddScopeToAppAsync(int scopeId, long appId)
        {
            ScopeApp scopeApp = new ScopeApp
            {
                ScopeId = scopeId,
                AppId = appId
            };

            db.Set<ScopeApp>().Add(scopeApp);
            await db.SaveChangesAsync();
        }

        public async Task<App> FindByNameAsync(string name)
        {
            return await db.Set<App>().SingleOrDefaultAsync(a => a.Name == name);
        }

        public IQueryable<App> GetQueryableApps(int userId)
        {
            return db.Set<App>().Where(a => a.UserId == userId).AsQueryable();
        }

        public IQueryable<IScope> GetQueryableScopes(long? appId = null)
        {
            return (appId.HasValue) 
                ? db.Set<ScopeApp>().Include(sa => sa.Scope).Where(sa => sa.AppId == appId).Select(sa => sa.Scope) 
                : db.Set<Scope>().AsQueryable();
        }

        public async Task RemoveScopeFromAppAsync(int scopeId, long appId)
        {
            ScopeApp scopeApp = await db.Set<ScopeApp>().SingleOrDefaultAsync(sa => sa.ScopeId == scopeId && sa.AppId == appId);

            if (scopeApp != null)
            {
                db.Set<ScopeApp>().Remove(scopeApp);
                await db.SaveChangesAsync();
            }
        }
    }
}
