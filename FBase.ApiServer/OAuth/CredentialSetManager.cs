using FBase.Foundations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public class CredentialSetManager<TCredentialSet> : ManagerBase<TCredentialSet, long>
        where TCredentialSet : class, ICredentialSet, new()
    {
        public CredentialSetManager(ICredentialSetStore<TCredentialSet> store) : base(store)
        {
        }

        private ICredentialSetStore<TCredentialSet> GetCredentialSetStore()
        {
            return (ICredentialSetStore<TCredentialSet>)Store;
        }

        public async Task<TCredentialSet> FindByNameAsync(string name, long appId)
        {
            return await GetCredentialSetStore().FindAsync(name, appId);
        }

        private async Task<TCredentialSet> FindByUniqueAsync(TCredentialSet match)
        {
            return await GetCredentialSetStore().FindAsync(match.Name, match.AppId);
        }

        public List<TCredentialSet> GetCredentialSets(long appId)
        {
            return GetCredentialSetStore().GetQueryableCredentialSets().Where(cs => cs.AppId == appId).OrderBy(cs => cs.Name).ToList();
        }

        public async Task<ManagerResult> CreateAsync(string name, long appId, ICredentialValuesProvider credentialValuesProvider = null)
        {
            credentialValuesProvider = credentialValuesProvider ?? new DummyCredentialValuesProvider();

            TCredentialSet credentialSet = new TCredentialSet();
            credentialSet.Name = name;
            credentialSet.ClientId = credentialValuesProvider.GenerateClientId();
            credentialSet.ClientSecret = credentialValuesProvider.GenerateClientSecret();
            credentialSet.AppId = appId;

            return await DataUtils.CreateAsync(
                entity: credentialSet,
                store: GetCredentialSetStore(),
                findUniqueAsync: FindByUniqueAsync);
        }

        private Func<TCredentialSet, ManagerResult> GenerateCanManipulateFunction<TUserKey>(TUserKey requestorId)
            where TUserKey : IEquatable<TUserKey>
        {
            return (credentialSet) =>
            {
                var app = GetCredentialSetStore().FindAppAsync<TUserKey>(credentialSet.AppId).Result;

                if (app == null)
                    return new ManagerResult(ApiServerMessages.CredentialSetNotFound);

                if (!app.UserId.Equals(requestorId))
                    return new ManagerResult(ManagerErrors.Unauthorized);

                return new ManagerResult();
            };
            
        }

        public async Task<ManagerResult> UpdateAsync<TUserKey>(long id, string name, TUserKey requestorId)
            where TUserKey: IEquatable<TUserKey>
        {
            return await DataUtils.UpdateAsync(
                id: id,
                store: GetCredentialSetStore(),
                findUniqueAsync: FindByUniqueAsync,                
                canUpdate: GenerateCanManipulateFunction(requestorId),
                fillNewValues: (originalCredentialSet) => {
                    originalCredentialSet.Name = name;
                });
        }

        public async Task<ManagerResult> UpdateAsync<TUserKey>(long id, TUserKey requestorId, ICredentialValuesProvider credentialValuesProvider = null)
            where TUserKey : IEquatable<TUserKey>
        {
            credentialValuesProvider = credentialValuesProvider ?? new DummyCredentialValuesProvider();


            return await DataUtils.UpdateAsync(
                id: id,
                store: GetCredentialSetStore(),
                findUniqueAsync: FindByUniqueAsync,
                canUpdate: GenerateCanManipulateFunction(requestorId),
                fillNewValues: (originalCredentialSet) => {
                    originalCredentialSet.ClientId = credentialValuesProvider.GenerateClientId();
                    originalCredentialSet.ClientSecret = credentialValuesProvider.GenerateClientSecret();
                });
        }

        public async Task<ManagerResult> DeleteAsync<TUserKey>(long id, TUserKey requestorId)
            where TUserKey : IEquatable<TUserKey>
        {
            return await DataUtils.DeleteAsync(
                id: id,
                store: GetCredentialSetStore(),
                canDelete: GenerateCanManipulateFunction(requestorId));
        }
    }
}
