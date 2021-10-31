using FBase.Foundations;
using System;
using System.Threading.Tasks;

namespace FBase.ApiServer
{
    public interface IRefreshTokenStore<TRefreshToken, TUserKey> : IAsyncStore<TRefreshToken, long>
       where TRefreshToken : class, IRefreshToken<TUserKey>
       where TUserKey : IEquatable<TUserKey>
    {
        Task<TRefreshToken> FindByTokenAsync(string token);
    }
}
