using FBase.DotNetCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FBase.Api.EntityFramework;

public class CredentialSetStore<TUser, TUserKey> : EntityStoreBase<CredentialSet<TUser, TUserKey>, long>, ICredentialSetStore<CredentialSet<TUser, TUserKey>, TUserKey>
    where TUser : IdentityUser<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    public CredentialSetStore(DbContext context) : base(context)
    {
    }

    public async Task<CredentialSet<TUser, TUserKey>> FindAsync(string name, long appId)
    {
        return await db.Set<CredentialSet<TUser, TUserKey>>().SingleOrDefaultAsync(cs => cs.Name == name && cs.AppId == appId);
    }

    public async Task<CredentialSet<TUser, TUserKey>> FindByClientIdAsync(string clientId)
    {
        return await db.Set<CredentialSet<TUser, TUserKey>>().SingleOrDefaultAsync(cs => cs.ClientId == clientId);
    }

    public async Task<IApp<TUserKey>> FindAppAsync(long appId)
    {
        return await db.Set<App<TUser, TUserKey>>().SingleOrDefaultAsync(a => a.Id.Equals(appId));
    }

    public IQueryable<CredentialSet<TUser, TUserKey>> GetQueryableCredentialSets()
    {
        return db.Set<CredentialSet<TUser, TUserKey>>().AsQueryable();
    }
}
