using FBase.Foundations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public interface ICredentialSetStore<TCredentialSet> : IAsyncStore<TCredentialSet, long>
        where TCredentialSet : class, ICredentialSet
    {
        IQueryable<TCredentialSet> GetQueryableCredentialSets();
        Task<TCredentialSet> FindAsync(string name, long appId);

        Task<IApp<TUserKey>> FindAppAsync<TUserKey>(long appId) where TUserKey : IEquatable<TUserKey>;
    }
}
