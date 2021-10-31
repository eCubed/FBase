using ApiServerLibraryTest.Data;
using ApiServerLibraryTest.Models;
using FBase.ApiServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ApiServerLibraryTestConfig Config { get; set; }
        private UserManager<TestUser> UserManager { get; set; }
        private RefreshTokenManager<RefreshToken, int> RefreshTokenManager { get; set; }

        private TokenValidationParameters TokenValidationParameters { get; set; }

        public AuthController(
            ApiServerLibraryTestConfig config, 
            UserManager<TestUser> userManager,
            ApiServerLibraryTestDbContext context,
            TokenValidationParameters tokenValidationParameters)
        {
            Config = config;
            UserManager = userManager;
            RefreshTokenManager = new RefreshTokenManager<RefreshToken, int>(new RefreshTokenStore(context));
            TokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            var res = await AuthenticationTools.LoginAsync<TestUser, int, RefreshToken, TokenResponse>(
                loginModel,
                Config,
                UserManager,
                RefreshTokenManager);

            if (!res.Success)
                return this.DiscernErrorActionResult(res);

            return Ok(res.Data);
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var res = await AuthenticationTools.VerifyAndGenerateTokenAsync<TestUser, int, RefreshToken, TokenResponse>(
                refreshTokenRequest: refreshTokenRequest,
                config: Config,
                refreshTokenManager: RefreshTokenManager,
                userManager: UserManager,
                tokenValidationParameters: TokenValidationParameters);

            return Ok(res);
        }
    }
}
