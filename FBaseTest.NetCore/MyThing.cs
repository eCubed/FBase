using FBase.Foundations;

namespace FBaseTest.NetCore
{
    public class MyThing : IIdentifiable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
