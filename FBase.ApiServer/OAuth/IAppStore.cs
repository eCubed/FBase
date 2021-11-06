using FBase.Foundations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public interface IAppStore<TApp, TUserKey> : IAsyncStore<TApp, long>
        where TApp : class, IApp<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        Task<TApp> FindByNameAsync(string name);
        IQueryable<TApp> GetQueryableApps(TUserKey userId);
    }
}
