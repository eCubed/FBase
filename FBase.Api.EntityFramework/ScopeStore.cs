using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FBase.Api.EntityFramework;

public class ScopeStore : EntityStoreBase<Scope, int>, IScopeStore<Scope>
{
    public ScopeStore(DbContext context) : base(context)
    {
    }

    public async Task<Scope?> FindByNameAsync(string name)
    {
        return await db.Set<Scope>().SingleOrDefaultAsync(s => s.Name == name);
    }

    public IQueryable<Scope> GetQueryableScopes()
    {
        return db.Set<Scope>().AsQueryable();
    }
}
