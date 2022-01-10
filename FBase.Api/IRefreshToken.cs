using FBase.Foundations;
using System;

namespace FBase.Api
{
    public interface IRefreshToken<TUserKey> : IIdentifiable<long>
        where TUserKey : IEquatable<TUserKey>
    {
        string Token { get; set; }
        string JwtId { get; set; }
        bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public TUserKey UserId { get; set; }
    }
}
