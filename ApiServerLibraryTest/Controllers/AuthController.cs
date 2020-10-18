using ApiServerLibraryTest.Data;
using ApiServerLibraryTest.Models;
using FBase.ApiServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ApiServerLibraryTestConfig Config { get; set; }
        private UserManager<TestUser> UserManager { get; set; }

        public AuthController(ApiServerLibraryTestConfig config, UserManager<TestUser> userManager)
        {
            Config = config;
            UserManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            var res = await AuthenticationTools.LoginAsync<TestUser, int, TokenResponse>(
                loginModel, UserManager, Config);

            if (!res.Success)
                return this.DiscernErrorActionResult(res);

            return Ok(res.Data);
        }
    }
}
