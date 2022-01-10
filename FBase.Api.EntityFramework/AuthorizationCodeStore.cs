using FBase.Api;
using FBase.DotNetCore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.Api.EntityFramework
{
    public class AuthorizationCodeStore<TUser, TUserKey> : EntityStoreBase<AuthorizationCode<TUser, TUserKey>, long>, IAuthorizationCodeStore<AuthorizationCode<TUser, TUserKey>, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public AuthorizationCodeStore(DbContext context) : base(context)
        {
        }

        public async Task DeleteAllExpiredBeforeAsync(DateTime expiredBeforeDate)
        {
            var deleteList = await db.Set<AuthorizationCode<TUser, TUserKey>>().Where(ac => ac.CreatedDate < expiredBeforeDate).ToListAsync();
            db.Set<AuthorizationCode<TUser, TUserKey>>().RemoveRange(deleteList);
            await db.SaveChangesAsync();
        }

        public async Task<AuthorizationCode<TUser, TUserKey>> FindByCodeAsync(string code)
        {
            return await db.Set<AuthorizationCode<TUser, TUserKey>>().SingleOrDefaultAsync(ac => ac.Code == code);
        }
    }
}
