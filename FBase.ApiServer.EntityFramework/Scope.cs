using FBase.ApiServer.OAuth;

namespace FBase.ApiServer.EntityFramework
{
    public class Scope : IScope
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Id { get; set; }
    }
}
