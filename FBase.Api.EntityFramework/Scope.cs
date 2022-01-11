using FBase.Api;

namespace FBase.Api.EntityFramework
{
    public class Scope : IScope
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public int Id { get; set; }
    }
}
