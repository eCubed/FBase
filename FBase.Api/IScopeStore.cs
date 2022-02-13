using FBase.Foundations;

namespace FBase.Api;

public interface IScopeStore<TScope> : IAsyncStore<TScope, int>
    where TScope : class, IScope
{
    IQueryable<TScope> GetQueryableScopes();
    Task<TScope?> FindByNameAsync(string name);
}
