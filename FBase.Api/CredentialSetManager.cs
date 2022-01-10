using FBase.Foundations;

namespace FBase.Api;

public class CredentialSetManager<TCredentialSet, TUserKey> : ManagerBase<TCredentialSet, long>
    where TCredentialSet : class, ICredentialSet, new()
    where TUserKey : IEquatable<TUserKey>
{
    public CredentialSetManager(ICredentialSetStore<TCredentialSet, TUserKey> store) : base(store)
    {
    }

    private ICredentialSetStore<TCredentialSet, TUserKey> GetCredentialSetStore()
    {
        return (ICredentialSetStore<TCredentialSet, TUserKey>)Store;
    }

    public async Task<TCredentialSet> FindAsync(string name, long appId)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await GetCredentialSetStore().FindAsync(name, appId);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async Task<TCredentialSet> FindByClientIdAsync(string clientId)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await GetCredentialSetStore().FindByClientIdAsync(clientId);
#pragma warning restore CS8603 // Possible null reference return.
    }

    private async Task<TCredentialSet> FindUniqueAsync(TCredentialSet match)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await GetCredentialSetStore().FindAsync(match.Name, match.AppId);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public List<TCredentialSet> GetCredentialSets(long appId)
    {
        return GetCredentialSetStore().GetQueryableCredentialSets().Where(cs => cs.AppId == appId).OrderBy(cs => cs.Name).ToList();
    }

    public async Task<ManagerResult> CreateAsync(string name, long appId, string redirectUrl, ICredentialValuesProvider credentialValuesProvider = null)
    {
        credentialValuesProvider = credentialValuesProvider ?? new DummyCredentialValuesProvider();

        TCredentialSet credentialSet = new TCredentialSet();
        credentialSet.Name = name;
        credentialSet.ClientId = credentialValuesProvider.GenerateClientId();
        credentialSet.ClientSecret = credentialValuesProvider.GenerateClientSecret();
        credentialSet.RedirectUrl = redirectUrl;
        credentialSet.AppId = appId;

        return await DataUtils.CreateAsync(
            entity: credentialSet,
            store: GetCredentialSetStore(),
            findUniqueAsync: FindUniqueAsync);
    }

    private Func<TCredentialSet, ManagerResult> GenerateCanManipulateFunction(TUserKey requestorId)
    {
        return (credentialSet) =>
        {
            var app = GetCredentialSetStore().FindAppAsync(credentialSet.AppId).Result;

            if (app == null)
                return new ManagerResult(ApiMessages.CredentialSetNotFound);

            if (!app.UserId.Equals(requestorId))
                return new ManagerResult(ManagerErrors.Unauthorized);

            return new ManagerResult();
        };

    }

    public async Task<ManagerResult> UpdateAsync(long id, string name, string redirectUrl, TUserKey requestorId)
    {
        return await DataUtils.UpdateAsync(
            id: id,
            store: GetCredentialSetStore(),
            findUniqueAsync: FindUniqueAsync,
            canUpdate: GenerateCanManipulateFunction(requestorId),
            fillNewValues: (originalCredentialSet) =>
            {
                originalCredentialSet.Name = name;
                originalCredentialSet.RedirectUrl = redirectUrl;
            });
    }

    public async Task<ManagerResult> UpdateAsync(long id, TUserKey requestorId, ICredentialValuesProvider credentialValuesProvider = null)
    {
        credentialValuesProvider = credentialValuesProvider ?? new DummyCredentialValuesProvider();


        return await DataUtils.UpdateAsync(
            id: id,
            store: GetCredentialSetStore(),
            findUniqueAsync: FindUniqueAsync,
            canUpdate: GenerateCanManipulateFunction(requestorId),
            fillNewValues: (originalCredentialSet) =>
            {
                originalCredentialSet.ClientId = credentialValuesProvider.GenerateClientId();
                originalCredentialSet.ClientSecret = credentialValuesProvider.GenerateClientSecret();
            });
    }

    public async Task<ManagerResult> DeleteAsync(long id, TUserKey requestorId)
    {
        return await DataUtils.DeleteAsync(
            id: id,
            store: GetCredentialSetStore(),
            canDelete: GenerateCanManipulateFunction(requestorId));
    }
}
