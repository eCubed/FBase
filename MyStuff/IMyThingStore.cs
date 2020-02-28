using FBase.Foundations;
using System.Linq;
using System.Threading.Tasks;

namespace MyStuff
{
    public interface IMyThingStore<TMyThing> : IAsyncStore<TMyThing, int>
        where TMyThing : class, IMyThing
    {
        Task<TMyThing> FindByNameAsync(string name);
        IQueryable<TMyThing> GetQueryableMyThings();
    }
}
