using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class AppAuthorizationStore : EntityStoreBase<AppAuthorization, long>, IAppAuthorizationStore<AppAuthorization, int>
    {
        public AppAuthorizationStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public async Task AddScopeToAppAuthorizationAsync(int scopeId, long appAuthorizationId)
        {
            ScopeAppAuthorization scopeAppAuthorization = new ScopeAppAuthorization
            {
                ScopeId = scopeId,
                AppAuthorizationId = appAuthorizationId
            };

            db.Set<ScopeAppAuthorization>().Add(scopeAppAuthorization);
            await db.SaveChangesAsync();
        }

        public async Task<AppAuthorization> FindUniqueAsync(long appId, int userId)
        {
            return await db.Set<AppAuthorization>().SingleOrDefaultAsync(aa => aa.AppId == appId && aa.UserId == userId);
        }

        public IQueryable<AppAuthorization> GetQueryableAppAuthorization()
        {
            return db.Set<AppAuthorization>().AsQueryable();
        }

        public IQueryable<IScope> GetQueryableAppAuthorizationScopes(long? appAuthorizationId = null)
        {
            return (appAuthorizationId.HasValue)
              ? db.Set<ScopeAppAuthorization>().Include(sa => sa.Scope).Where(sa => sa.AppAuthorizationId == appAuthorizationId).Select(sa => sa.Scope)
              : db.Set<Scope>().AsQueryable();
        }

        public IQueryable<IScope> GetQueryableAppScopes(long? appId)
        {
            return (appId.HasValue)
               ? db.Set<ScopeApp>().Include(sa => sa.Scope).Where(sa => sa.AppId == appId).Select(sa => sa.Scope)
               : db.Set<Scope>().AsQueryable();
        }
    }
}
