using FBase.Api;
using Microsoft.AspNetCore.Identity;
using System;

namespace FBase.Api.EntityFramework
{
    public class AppAuthorization<TUser, TUserKey> : IAppAuthorization<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>

    {
        public long AppId { get; set; }
        public App<TUser, TUserKey> App { get; set; } = null!;
        public TUserKey UserId { get; set; } = default!;
        public TUser User { get; set; } = null!;

        public long Id { get; set; }
    }
}
