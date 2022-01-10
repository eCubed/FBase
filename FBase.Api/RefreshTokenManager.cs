using FBase.Foundations;

namespace FBase.Api
{
    public class RefreshTokenManager<TRefreshToken, TUserKey> : ManagerBase<TRefreshToken, long>
        where TRefreshToken : class, IRefreshToken<TUserKey>, new()
        where TUserKey : IEquatable<TUserKey>
    {
        public RefreshTokenManager(IRefreshTokenStore<TRefreshToken, TUserKey> store) : base(store)
        {
        }

        private IRefreshTokenStore<TRefreshToken, TUserKey> GetRefreshTokenStore()
        {
            return (IRefreshTokenStore<TRefreshToken, TUserKey>)Store;
        }

        public async Task<ManagerResult<TRefreshToken>> CreateAsync(string jwtId, TUserKey userId)
        {
            TRefreshToken refreshToken = new TRefreshToken();
            refreshToken.JwtId = jwtId;
            refreshToken.IsRevoked = false;
            refreshToken.UserId = userId;
            refreshToken.CreatedDate = DateTime.UtcNow;
            refreshToken.ExpirationDate = DateTime.UtcNow.AddMonths(6);
            refreshToken.Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var createRes = await DataUtils.CreateAsync(
                entity: refreshToken,
                store: GetRefreshTokenStore());

            if (!createRes.Success)
            {
                return new ManagerResult<TRefreshToken>(createRes.Errors);
            }

            return new ManagerResult<TRefreshToken>(refreshToken);
        }

        public async Task<TRefreshToken> FindByTokenAsync(string token)
        {
            return await GetRefreshTokenStore().FindByTokenAsync(token);
        }
    }
}
