using FBase.DotNetCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FBase.ApiServer.EntityFramework
{
    public class RefreshTokenStore<TUser, TUserKey> : EntityStoreBase<RefreshToken<TUser, TUserKey>, long>, IRefreshTokenStore<RefreshToken<TUser, TUserKey>, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public RefreshTokenStore(DbContext context) : base(context)
        {
        }

        public async Task<RefreshToken<TUser, TUserKey>> FindByTokenAsync(string token)
        {
            return await db.Set<RefreshToken<TUser, TUserKey>>().SingleOrDefaultAsync(rt => rt.Token == token);
        }
    }
}
