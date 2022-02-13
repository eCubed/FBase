using FBase.Foundations;

namespace FBase.Api;

public class AuthorizationCodeManager<TAuthorizationCode, TUserKey> : ManagerBase<TAuthorizationCode, long>
    where TAuthorizationCode : class, IAuthorizationCode<TUserKey>, new()
    where TUserKey : IEquatable<TUserKey>
{
    public AuthorizationCodeManager(IAuthorizationCodeStore<TAuthorizationCode, TUserKey> store) : base(store)
    {
    }

    private IAuthorizationCodeStore<TAuthorizationCode, TUserKey> GetAuthorizationCodeStore()
    {
        return (IAuthorizationCodeStore<TAuthorizationCode, TUserKey>)Store;
    }

    public async Task<TAuthorizationCode?> FindByCodeAsync(string code)
    {
        return await GetAuthorizationCodeStore().FindByCodeAsync(code);
    }

    public async Task<ManagerResult<TAuthorizationCode>> CreateAsync(string codeChallenge, TUserKey userId, long appId)
    {
        TAuthorizationCode authorizationCode = new TAuthorizationCode
        {
            CodeChallenge = codeChallenge,
            UserId = userId,
            AppId = appId,
            Code = Randomizer.GenerateString(Randomizer.GenerateRandomInteger(8, 10), "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjklmnopqrstuvwxyz0123456789") + "-" +
                   Randomizer.GenerateString(Randomizer.GenerateRandomInteger(11, 17), "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjklmnopqrstuvwxyz0123456789")
        };

        var createRes = await DataUtils.CreateAsync(
            entity: authorizationCode,
            store: GetAuthorizationCodeStore());

        if (!createRes.Success)
            return new ManagerResult<TAuthorizationCode>(createRes.Errors);

        return new ManagerResult<TAuthorizationCode>(authorizationCode);
    }


    public async Task<ManagerResult<TAuthorizationCode>> ValidateAsync(string code, string codeVerifier, int gracePeriodInMinutes = 5)
    {
        TAuthorizationCode? authorizationCode = await GetAuthorizationCodeStore().FindByCodeAsync(code);

        if (authorizationCode == null)
            return new ManagerResult<TAuthorizationCode>(ApiMessages.InvalidAuthorizationCode);

        if (authorizationCode.CreatedDate?.AddMinutes(gracePeriodInMinutes) < DateTime.Now)
            return new ManagerResult<TAuthorizationCode>(ApiMessages.ExpiredAuthorizationCode);

        if (!PckeUtils.Check(codeVerifier, authorizationCode.CodeChallenge))
            return new ManagerResult<TAuthorizationCode>(ApiMessages.InvalidCodeVerifier);

        await GetAuthorizationCodeStore().DeleteAsync(authorizationCode);

        return new ManagerResult<TAuthorizationCode>(authorizationCode);
    }

    public async Task DeleteAllExpiredAsync(int gracePeriodInMinutes = 5)
    {
        await GetAuthorizationCodeStore().DeleteAllExpiredBeforeAsync(DateTime.Now.AddMinutes(-1 * gracePeriodInMinutes));
    }
}
