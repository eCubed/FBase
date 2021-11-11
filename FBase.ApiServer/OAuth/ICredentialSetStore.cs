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
#nullable enable
        Task<TCredentialSet?> FindAsync(string name, long appId);
#nullable enable
        Task<TCredentialSet?> FindByClientIdAsync(string clientId);
#nullable enable
        Task<IApp<TUserKey>?> FindAppAsync<TUserKey>(long appId) where TUserKey : IEquatable<TUserKey>;
    }
}
