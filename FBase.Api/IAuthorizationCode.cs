using FBase.Foundations;
using System;

namespace FBase.Api
{
    public interface IAuthorizationCode<TUserKey> : IIdentifiable<long>
        where TUserKey : IEquatable<TUserKey>
    {
        string Code { get; set; }
        TUserKey UserId { get; set; }
        long AppId { get; set; }
        string CodeChallenge { get; set; }
        DateTime? CreatedDate { get; set; }

    }
}
