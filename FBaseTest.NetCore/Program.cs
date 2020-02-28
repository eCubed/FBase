using System;

namespace FBaseTest.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            MyThing myThing = new MyThing { Id = 12, Name = "Thingamajig", Description = "Just Something" };

            Console.WriteLine("My thing is " + myThing.Name + " that is " + myThing.Description);

            Console.ReadLine();
        }
    }
}
