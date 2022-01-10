using FBase.Foundations;

namespace FBase.Api;

public interface ICredentialSetStore<TCredentialSet, TUserKey> : IAsyncStore<TCredentialSet, long>
    where TCredentialSet : class, ICredentialSet
    where TUserKey : IEquatable<TUserKey>
{
    IQueryable<TCredentialSet> GetQueryableCredentialSets();

    Task<TCredentialSet?> FindAsync(string name, long appId);

    Task<TCredentialSet?> FindByClientIdAsync(string clientId);

    Task<IApp<TUserKey>?> FindAppAsync(long appId);
}
