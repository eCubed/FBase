namespace FBase.Api.Server.Models;

public class AuthenticatedInfo<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    public TUserKey? UserId { get; set; } = default!;
}
