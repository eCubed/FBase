namespace FBase.Foundations;

public abstract class ManagerBase<T, TKey>
    where T : class, IIdentifiable<TKey>
{
    protected IAsyncStore<T, TKey> Store { get; set; }

    public ManagerBase(IAsyncStore<T, TKey> store)
    {
        Store = store;
    }

    public virtual async Task<T?> FindByIdAsync(TKey id)
    {
        return await Store.FindByIdAsync(id);
    }
}
