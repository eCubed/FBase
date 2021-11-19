using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBase.ApiServer.EntityFramework
{
    public class AuthorizationCode<TUser, TUserKey> : IAuthorizationCode<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public string Code { get; set; }
        public TUserKey UserId { get; set; }
        public TUser User { get; set; }
        public long AppId { get; set; }
        public App<TUser, TUserKey> App { get; set; }
        public string CodeChallenge { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}
