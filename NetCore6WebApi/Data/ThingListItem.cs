using FBase.Foundations;

namespace NetCore6WebApi.Data;

public class ThingListItem<TThing> : IDisplayModel<TThing>
    where TThing : class, IThing
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ThingListItem()
    {
    }

    public ThingListItem(TThing thing)
    {
        FillModel(thing);
    }
    public void FillModel(TThing thing)
    {
        Id = thing.Id;
        Name = thing.Name;
    }
}
