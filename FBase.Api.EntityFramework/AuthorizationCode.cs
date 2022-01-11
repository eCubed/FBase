using FBase.Api;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBase.Api.EntityFramework
{
    public class AuthorizationCode<TUser, TUserKey> : IAuthorizationCode<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public string Code { get; set; } = "";
        public TUserKey UserId { get; set; } = default!;
        public TUser User { get; set; } = null!;
        public long AppId { get; set; }
        public App<TUser, TUserKey> App { get; set; } = null!;
        public string CodeChallenge { get; set; } = "";
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}
