using System.Threading.Tasks;

namespace FBase.Foundations
{
    public interface IAsyncStore<T, in TKey>
        where T : class, IIdentifiable<TKey>
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(TKey id);
        Task DeleteAsync(T entity);
        Task<T> FindByIdAsync(TKey id);
    }
}
