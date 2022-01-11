using Microsoft.AspNetCore.Identity;
using System;

namespace FBase.Api.EntityFramework
{
    public class ScopeApp<TUser, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public long Id { get; set; }
        public int ScopeId { get; set; }
        public Scope Scope { get; set; } = null!;
        public long AppId { get; set; }
        public App<TUser, TUserKey> App { get; set; } = null!;
    }
}
