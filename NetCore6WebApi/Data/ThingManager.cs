using FBase.Foundations;

namespace NetCore6WebApi.Data
{
    public class ThingManager<TThing> : ManagerBase<TThing, int>
        where TThing : class, IThing, new()
    {
        public ThingManager(IThingStore<TThing> store) : base(store)
        {
        }
    }
}
