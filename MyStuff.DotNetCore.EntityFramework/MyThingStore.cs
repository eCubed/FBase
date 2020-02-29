using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MyStuff.DotNetCore.EntityFramework
{
    public class MyThingStore : EntityStoreBase<MyThing, int>, IMyThingStore<MyThing>
    {
        public MyThingStore(MyStuffDbContext context) : base(context)
        {
        }

        public async Task<MyThing> FindByNameAsync(string name)
        {
            return await db.Set<MyThing>().SingleOrDefaultAsync(t => t.Name == name);
        }

        public IQueryable<MyThing> GetQueryableMyThings()
        {
            return db.Set<MyThing>().AsQueryable();
        }
    }
}
