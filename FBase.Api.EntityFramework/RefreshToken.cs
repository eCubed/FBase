﻿using Microsoft.AspNetCore.Identity;
using System;

namespace FBase.Api.EntityFramework
{
    public class RefreshToken<TUser, TUserKey> : IRefreshToken<TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        public string Token { get; set; } = "";
        public string JwtId { get; set; } = "";
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public TUserKey UserId { get; set; } = default!;
        public TUser User { get; set; } = null!;
        public long Id { get; set; }
    }
}
