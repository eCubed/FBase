using FBase.Foundations;

namespace FBase.ApiServer.OAuth
{
    public interface IScope : IIdentifiable<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
