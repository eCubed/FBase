using FBase.Foundations;

namespace NetCore6WebApi.Data
{
    public interface IThingStore<TThing> : IAsyncStore<TThing, int>
        where TThing : class, IThing
    {
        IQueryable<TThing> GetQueryableThings();
        Task<TThing?> FindByNameAsync(string name);
    }
}
