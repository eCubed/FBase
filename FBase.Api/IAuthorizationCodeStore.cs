using FBase.Foundations;

namespace FBase.Api;

public interface IAuthorizationCodeStore<TAuthorizationCode, TUserKey> : IAsyncStore<TAuthorizationCode, long>
    where TAuthorizationCode : class, IAuthorizationCode<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
#nullable enable
    Task<TAuthorizationCode?> FindByCodeAsync(string code);
    Task DeleteAllExpiredBeforeAsync(DateTime expiredBeforeDate);
}
