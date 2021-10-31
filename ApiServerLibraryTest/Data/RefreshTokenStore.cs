using FBase.ApiServer;
using FBase.DotNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class RefreshTokenStore : EntityStoreBase<RefreshToken, long>, IRefreshTokenStore<RefreshToken, int>
    {
        public RefreshTokenStore(ApiServerLibraryTestDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken> FindByTokenAsync(string token)
        {
            return await db.Set<RefreshToken>().SingleOrDefaultAsync(rt => rt.Token == token);
        }
    }
}
