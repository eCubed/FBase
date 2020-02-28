using FBase.Foundations;

namespace FBaseTest.Net45x
{
    public class MyThing : IIdentifiable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
