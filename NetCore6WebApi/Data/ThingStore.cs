using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace NetCore6WebApi.Data
{
    public class ThingStore : EntityStoreBase<Thing, int>, IThingStore<Thing>
    {
        public ThingStore(TestingDbContext context) : base(context)
        {
        }

        public async Task<Thing?> FindByNameAsync(string name)
        {
            return await db.Set<Thing>().SingleOrDefaultAsync(t => t.Name == name);
        }

        public IQueryable<Thing> GetQueryableThings()
        {
            return db.Set<Thing>().AsQueryable();
        }
    }
}
