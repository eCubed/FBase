using FBase.Foundations;

namespace FBase.Api;

public interface IAppUser<TUserKey> : IIdentifiable<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    string UserName { get; set; }
    string Email { get; set; }
}
