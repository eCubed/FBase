using FBase.Foundations;
using System;

namespace FBase.ApiServer.OAuth
{
    public interface IAppAuthorization<TUserKey> : IIdentifiable<long>
        where TUserKey : IEquatable<TUserKey>
    {
        long AppId { get; set; }
        TUserKey UserId { get; set; }
    }
}
