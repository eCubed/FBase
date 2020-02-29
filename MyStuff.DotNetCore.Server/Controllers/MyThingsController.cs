using Microsoft.AspNetCore.Mvc;
using MyStuff.DotNetCore.EntityFramework;
using MyStuff.DotNetCore.Server.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyStuff.DotNetCore.Server.Controllers
{
    [Route("api/[controller]")]
    public class MyThingsController : Controller
    {
        private MyStuffDbContext db { get; set; }
        private MyThingManager<MyThing> MyThingManager { get; set; }

        public MyThingsController(MyStuffDbContext context)
        {
            db = context;
            MyThingManager = new MyThingManager<MyThing>(new MyThingStore(db));
        }

        // GET: api/<controller>
        [HttpGet("{page}/{pageSize}")]
        public IActionResult Get(int page = 1, int pageSize = 10)
        {
            var res = MyThingManager.GetMyThings<MyThingViewModel>(page, pageSize);

            return Ok(res);
        }
    }
}
