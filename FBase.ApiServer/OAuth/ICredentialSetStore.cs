using FBase.Foundations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public interface ICredentialSetStore<TCredentialSet, TUserKey> : IAsyncStore<TCredentialSet, long>
        where TCredentialSet : class, ICredentialSet
        where TUserKey : IEquatable<TUserKey>
    {
        IQueryable<TCredentialSet> GetQueryableCredentialSets();
#nullable enable
        Task<TCredentialSet?> FindAsync(string name, long appId);
#nullable enable
        Task<TCredentialSet?> FindByClientIdAsync(string clientId);
#nullable enable
        Task<IApp<TUserKey>?> FindAppAsync(long appId);
    }
}
