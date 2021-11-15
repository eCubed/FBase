using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using System;

namespace FBase.ApiServer.EntityFramework
{
    public class AppAuthorization<TUser, TUserKey> : IAppAuthorization<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>

    {
        public long AppId { get; set; }
        public App<TUser, TUserKey> App { get; set; }
        public TUserKey UserId { get; set; }
        public TUser User { get; set; }

        public long Id { get; set; }
    }
}
