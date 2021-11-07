using FBase.ApiServer.OAuth;

namespace ApiServerLibraryTest.Data
{
    public class Scope : IScope
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Id { get; set; }
    }
}
