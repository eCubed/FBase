using MyStuff.DotNet.EntityFramework;
using MyStuff.DotNet.Server.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace MyStuff.DotNet.Server.Controllers
{
    public class MyThingsController : ApiController
    {
        private MyThingManager<MyThing> MyThingManager { get; set; }

        public MyThingsController()
        {
            MyStuffDbContext db = new MyStuffDbContext();
            MyThingManager = new MyThingManager<MyThing>(new MyThingStore(db));
        }

        // GET api/<controller>
        [HttpGet]
        [Route("api/mythings/{page}/{pageSize}")]
        public IHttpActionResult Get(int page = 1, int pageSize = 12)
        {
            var res = MyThingManager.GetMyThings<MyThingViewModel>(page, pageSize);

            return Ok(res);
        }
    }
}