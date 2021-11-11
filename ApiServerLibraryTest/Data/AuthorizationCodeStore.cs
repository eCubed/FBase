using FBase.ApiServer.OAuth;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ApiServerLibraryTest.Data
{
    public class AuthorizationCodeStore : EntityStoreBase<AuthorizationCode, long>, IAuthorizationCodeStore<AuthorizationCode, int>
    {
        public AuthorizationCodeStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public async Task DeleteAllExpiredBeforeAsync(DateTime expiredBeforeDate)
        {
            var deleteList = await db.Set<AuthorizationCode>().Where(ac => ac.CreatedDate < expiredBeforeDate).ToListAsync();
            db.Set<AuthorizationCode>().RemoveRange(deleteList);
            await db.SaveChangesAsync();
        }

        public async Task<AuthorizationCode?> FindByCodeAsync(string code)
        {
            return await db.Set<AuthorizationCode>().SingleOrDefaultAsync(ac => ac.Code == code);
        }
    }
}
