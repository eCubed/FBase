using FBase.Foundations;

namespace FBase.Api;

public interface IRefreshTokenStore<TRefreshToken, TUserKey> : IAsyncStore<TRefreshToken, long>
   where TRefreshToken : class, IRefreshToken<TUserKey>
   where TUserKey : IEquatable<TUserKey>
{
#nullable enable
    Task<TRefreshToken?> FindByTokenAsync(string token);
}
