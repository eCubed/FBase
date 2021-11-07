using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class ScopeStore : EntityStoreBase<Scope, int>, IScopeStore<Scope>
    {
        public ScopeStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public async Task<Scope> FindByNameAsync(string name)
        {
            return await db.Set<Scope>().SingleOrDefaultAsync(s => s.Name == name);
        }

        public IQueryable<Scope> GetQueryableScopes()
        {
            return db.Set<Scope>().AsQueryable();
        }
    }
}
