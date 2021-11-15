using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class CredentialSetStore<TUserKey> : EntityStoreBase<CredentialSet, long>, ICredentialSetStore<CredentialSet, TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public CredentialSetStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public Task<IApp<TUserKey>?> FindAppAsync(long appId)
        {
            throw new NotImplementedException();
        }

        public async Task<CredentialSet?> FindAsync(string name, long appId)
        {
            return await db.Set<CredentialSet>().SingleOrDefaultAsync(cs => cs.Name == name && cs.AppId == appId);
        }

        public async Task<CredentialSet?> FindByClientIdAsync(string clientId)
        {
            return await db.Set<CredentialSet>().SingleOrDefaultAsync(cs => cs.ClientId == clientId);
        }

        public IQueryable<CredentialSet> GetQueryableCredentialSets()
        {
            return db.Set<CredentialSet>().AsQueryable();
        }
    }
}
