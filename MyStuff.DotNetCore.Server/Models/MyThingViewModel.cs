
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using FBase.Foundations;
using MyStuff.DotNetCore.EntityFramework;

namespace MyStuff.DotNetCore.Server.Models
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
