using Microsoft.AspNetCore.Identity;
using System;

namespace FBase.Api.EntityFramework
{
    public class ScopeAppAuthorization<TUser, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public long Id { get; set; }
        public int ScopeId { get; set; }
        public Scope Scope { get; set; }
        public long AppAuthorizationId { get; set; }
        public AppAuthorization<TUser, TUserKey> AppAuthorization { get; set; }
    }
}
