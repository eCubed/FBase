using FBase.Foundations;

namespace MyStuff
{
    public interface IMyThing : IIdentifiable<int>
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
