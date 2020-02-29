using FBase.Foundations;
using MyStuff.DotNet.EntityFramework;

namespace MyStuff.DotNet.Server.Models
{
    public class MyThingViewModel : IViewModel<MyThing, int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Id { get; set; }

        public void FillViewModel(MyThing data)
        {
            Name = data.Name;
            Description = data.Description;
            Id = data.Id;
        }

        public void UpdateObject(MyThing data)
        {
            data.Name = Name;
            data.Description = Description;
        }
    }
}