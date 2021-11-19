using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBase.ApiServer.EntityFramework
{
    public class App<TUser, TUserKey> : IApp<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        public TUserKey UserId { get; set; }
        public TUser User { get; set; }
        public string Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public long Id { get; set; }
    }
}
