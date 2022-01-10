using FBase.Api;
using FBase.Api.EntityFramework;
using FBase.Api.Server.Models;
using FBase.Api.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetCore6WebApi.Data;

namespace NetCore6WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private TestingConfig Config { get; set; } = null!;
        private UserManager<TestingUser> UserManager { get; set; } = null!;

        private RefreshTokenManager<RefreshToken<TestingUser, string>, string> RefreshTokenManager { get; set; } = null!;
        private TokenValidationParameters TokenValidationParameters { get; set; } = null!;

        public AuthController(TestingDbContext db, TestingConfig config, UserManager<TestingUser> userManager, TokenValidationParameters tokenValidationParameters)
        {
            Config = config;
            UserManager = userManager;
            RefreshTokenManager = new RefreshTokenManager<RefreshToken<TestingUser, string>, string>(new RefreshTokenStore<TestingUser, string>(db));
            TokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            if (loginModel == null)
                return BadRequest();

            var res = await AuthenticationTools.LoginAsync(
                loginModel,
                Config,
                UserManager,
                RefreshTokenManager);

            return this.ToActionResult(res);
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var res = await AuthenticationTools.VerifyAndGenerateTokenAsync(
                refreshTokenRequest: refreshTokenRequest,
                config: Config,
                refreshTokenManager: RefreshTokenManager,
                userManager: UserManager,
                tokenValidationParameters: TokenValidationParameters);

            return Ok(res);
        }
    }
}
