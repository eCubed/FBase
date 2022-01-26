using FBase.Api.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCore6WebApi.Data;

namespace NetCore6WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ThingsController : ControllerBase
{
    private TestingDbContext db { get; set; }

    private ThingManager<Thing> ThingManager { get; set; }
    private UserManager<TestingUser> UserManager { get; set; }
    public ThingsController(TestingDbContext context, UserManager<TestingUser> userManager)
    {
        db = context;
        ThingManager = new ThingManager<Thing>(new ThingStore(db));
        UserManager = userManager;
    }

    [HttpPost]
    [Authorize(Roles = "subscriber")]
    public async Task<IActionResult> CreateAsync([FromBody] ThingModel<Thing> thingModel)
    {
        var authInfo = await this.ResolveAuthenticatedEntitiesAsync<TestingUser, string>(UserManager);
        var res = await ThingManager.CreateAsync(thingModel, authInfo.UserId!);
        return this.ToActionResult<dynamic>(res, new { id = res.Data }, System.Net.HttpStatusCode.Created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "subscriber")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ThingModel<Thing> thingModel)
    {
        var authInfo = await this.ResolveAuthenticatedEntitiesAsync<TestingUser, string>(UserManager);

        return this.ToActionResult(await ThingManager.UpdateAsync(id, thingModel, authInfo.UserId!));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "subscriber")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var authInfo = await this.ResolveAuthenticatedEntitiesAsync<TestingUser, string>(UserManager);

        return this.ToActionResult(await ThingManager.GetAsync(id, authInfo.UserId!));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "subscriber")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var authInfo = await this.ResolveAuthenticatedEntitiesAsync<TestingUser, string>(UserManager);
        return this.ToActionResult(await ThingManager.DeleteAsync(id, authInfo.UserId!));
    }

    [HttpGet("all/{page}/{pageSize}")]
    [Authorize(Roles = "subscriber")]
    public async Task<IActionResult> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var authInfo = await this.ResolveAuthenticatedEntitiesAsync<TestingUser, string>(UserManager);
        return Ok(ThingManager.GetThings(authInfo.UserId!, page, pageSize));
    }


}
