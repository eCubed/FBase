using FBase.Foundations;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public interface IScopeStore<TScope> : IAsyncStore<TScope, int>
        where TScope : class, IScope
    {
        IQueryable<TScope> GetQueryableScopes();
#nullable enable
        Task<TScope?> FindByNameAsync(string name);
    }
}
