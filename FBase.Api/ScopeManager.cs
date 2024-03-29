﻿using FBase.Foundations;

namespace FBase.Api;

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

    private async Task<TScope?> FindUniqueAsync(TScope match)
    {
        return await GetScopeStore().FindByNameAsync(match.Name);
    }

    public List<TScope> GetScopes()
    {
        return GetScopeStore().GetQueryableScopes().OrderBy(cs => cs.Name).ToList();
    }

    public async Task<ManagerResult> CreateAsync(string name, string description)
    {

        TScope scope = new()
        {
            Name = name,
            Description = description
        };

        return await DataUtils.CreateAsync(
            entity: scope,
            store: GetScopeStore(),
            findUniqueAsync: FindUniqueAsync);
    }

    public async Task<ManagerResult> UpdateAsync(int id, string name, string description)
    {
        return await DataUtils.UpdateAsync(
            id: id,
            store: GetScopeStore(),
            findUniqueAsync: FindUniqueAsync,
            fillNewValues: (originalScope) =>
            {
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
