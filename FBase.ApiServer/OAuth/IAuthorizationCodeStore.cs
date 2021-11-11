using FBase.Foundations;
using System;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public interface IAuthorizationCodeStore<TAuthorizationCode, TUserKey> : IAsyncStore<TAuthorizationCode, long>
        where TAuthorizationCode : class, IAuthorizationCode<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        Task<TAuthorizationCode> FindByCodeAsync(string code);
        Task DeleteAllExpiredBeforeAsync(DateTime expiredBeforeDate);
    }
}
