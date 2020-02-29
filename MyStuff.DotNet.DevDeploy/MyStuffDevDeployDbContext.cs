using MyStuff.DotNet.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStuff.DotNet.DevDeploy
{
    public class MyStuffDevDeployDbContext : MyStuffDbContext
    {
        public MyStuffDevDeployDbContext() : base()
        {
        }
    }
}