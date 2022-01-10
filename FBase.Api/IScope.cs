using FBase.Foundations;

namespace FBase.Api
{
    public interface IScope : IIdentifiable<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
