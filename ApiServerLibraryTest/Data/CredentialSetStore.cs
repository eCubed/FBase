using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class CredentialSetStore : EntityStoreBase<CredentialSet, long>, ICredentialSetStore<CredentialSet>
    {
        public CredentialSetStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public Task<IApp<TUserKey>> FindAppAsync<TUserKey>(long appId) where TUserKey : IEquatable<TUserKey>
        {
            throw new NotImplementedException();
        }

        public async Task<CredentialSet> FindAsync(string name, long appId)
        {
            return await db.Set<CredentialSet>().SingleOrDefaultAsync(cs => cs.Name == name && cs.AppId == appId);
        }

        public async Task<CredentialSet> FindByClientIdAsync(string clientId)
        {
            return await db.Set<CredentialSet>().SingleOrDefaultAsync(cs => cs.ClientId == clientId);
        }

        public async Task<IApp<int>> FindAppAsync(long appId)
        {
            return await db.Set<App>().SingleOrDefaultAsync(a => a.Id == appId);
        }

        public IQueryable<CredentialSet> GetQueryableCredentialSets()
        {
            return db.Set<CredentialSet>().AsQueryable();
        }
    }
}
