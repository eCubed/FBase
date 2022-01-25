using FBase.Foundations;

namespace NetCore6WebApi.Data;

public class ThingModel<TThing> : ISaveModel<TThing>, IDisplayModel<TThing>
    where TThing : class, IThing
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public ThingModel()
    {
    }

    public ThingModel(TThing thing)
    {
        FillModel(thing);
    }

    public void SetObjectValues(TThing thing)
    {
        thing.Name = Name;
        thing.Description = Description;
    }

    public void FillModel(TThing thing)
    {
        Name = thing.Name;
        Description = thing.Description;
    }
}
