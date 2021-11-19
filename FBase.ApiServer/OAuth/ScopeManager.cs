using FBase.Foundations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer.OAuth
{
    public class ScopeManager<TScope> : ManagerBase<TScope, int>
        where TScope : class, IScope, new()
    {
        public ScopeManager(IScopeStore<TScope> store) : base(store)
        {
        }

        private IScopeStore<TScope> GetScopeStore()
        {
            return (IScopeStore<TScope>)Store;
        }

        public async Task<TScope> FindByNameAsync(string name)
        {
            return await GetScopeStore().FindByNameAsync(name);
        }

        private async Task<TScope> FindUniqueAsync(TScope match)
        {
            return await GetScopeStore().FindByNameAsync(match.Name);
        }

        public List<TScope> GetScopes()
        {
            return GetScopeStore().GetQueryableScopes().OrderBy(cs => cs.Name).ToList();
        }

        public async Task<ManagerResult> CreateAsync(string name, string description)
        {

            TScope scope = new TScope();
            scope.Name = name;
            scope.Description = description;

            return await DataUtils.CreateAsync(
                entity: scope,
                store: GetScopeStore(),
                findUniqueAsync: FindUniqueAsync);
        }

        public async Task<ManagerResult> UpdateAsync(int id, string name, string description)
        {
            return await DataUtils.UpdateAsync<TScope, int>(
                id: id,
                store: GetScopeStore(),
                findUniqueAsync: FindUniqueAsync,
                fillNewValues: (originalScope) => {
                    originalScope.Name = name;
                });
        }

        public async Task<ManagerResult> DeleteAsync(int id)
        {
            return await DataUtils.DeleteAsync(
                id: id,
                store: GetScopeStore());
        }
    }
}
