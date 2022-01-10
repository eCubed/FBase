using FBase.Api;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBase.Api.EntityFramework
{
    public class CredentialSet<TUser, TUserKey> : ICredentialSet
        where TUser : IdentityUser<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        public long AppId { get; set; }
        public App<TUser, TUserKey> App { get; set; } = default!;
        public string Name { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string RedirectUrl { get; set; } = "";
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}
