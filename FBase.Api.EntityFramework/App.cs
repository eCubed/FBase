using FBase.Api;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBase.Api.EntityFramework
{
    public class App<TUser, TUserKey> : IApp<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        public TUserKey UserId { get; set; } = default!;
        public TUser User { get; set; } = null!;
        public string Name { get; set; } = "";
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}
