using FBase.Foundations;

namespace NetCore6WebApi.Data;

public class ThingManager<TThing> : ManagerBase<TThing, int>
    where TThing : class, IThing, new()
{
    public ThingManager(IThingStore<TThing> store) : base(store)
    {
    }

    private IThingStore<TThing> GetThingStore() => (IThingStore<TThing>)Store;

    private async Task<TThing?> FindUniqueAsync(TThing match)
    {
        return await GetThingStore().FindByNameAsync(match.Name);
    }

    public async Task<ManagerResult<int>> CreateAsync(ThingModel<TThing> thingModel, string userId)
    {
        return await DataUtils.CreateAsync(
            model: thingModel,
            store: GetThingStore(),
            findUniqueAsync: FindUniqueAsync,
            setNonModelData: (thing) =>
            {
                thing.UserId = userId;
            });
    }

    private Func<TThing, ManagerResult> GenerateAuthorizationFunction(string userId) =>
        (thing) =>
        {
            if (thing.UserId != userId)
                return new ManagerResult(ManagerErrors.Unauthorized);

            return new ManagerResult();
        };

    public async Task<ManagerResult> UpdateAsync(int id, ThingModel<TThing> thingModel, string userId)
    {
        return await DataUtils.UpdateAsync(
            id: id,
            store: GetThingStore(),
            findUniqueAsync: FindUniqueAsync,
            canUpdate: GenerateAuthorizationFunction(userId),
            fillNewValues: (thing) =>
            {
                thingModel.SetObjectValues(thing);
            });
    }

    public async Task<ManagerResult> DeleteAsync(int id, string userId)
    {
        return await DataUtils.DeleteAsync(
            id: id,
            store: GetThingStore(),
            canDelete: GenerateAuthorizationFunction(userId));
    }

    public ResultSet<ThingListItem<TThing>> GetThings(string userId, int page = 1, int pageSize = 10)
    {
        return  DataUtils.GetMany<TThing, int, ThingListItem<TThing>>(
            filteredQueryable: GetThingStore().GetQueryableThings().Where(t => t.UserId == userId).OrderBy(t => (t != null) ? t.Name : "").AsQueryable(),
            page: page,
            pageSize: pageSize);
    }

    public async Task<ManagerResult<ThingModel<TThing>>> GetAsync(int id, string userId)
    {
        return await DataUtils.GetOneRecordAsync<TThing, int, ThingModel<TThing>>(
            id: id,
            store: GetThingStore(),
            canGet: GenerateAuthorizationFunction(userId));
    }
    
}
