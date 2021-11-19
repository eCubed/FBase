using ApiServerLibraryTest.Data;
using ApiServerLibraryTest.Models;
using FBase.ApiServer;
using FBase.ApiServer.EntityFramework;
using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ApiServerLibraryTestConfig Config { get; set; }
        private UserManager<TestUser> UserManager { get; set; }
        private RefreshTokenManager<RefreshToken<TestUser, int>, int> RefreshTokenManager { get; set; }
        private AuthorizationCodeManager<AuthorizationCode<TestUser, int>, int> AuthorizationCodeManager { get; set; }

        private TokenValidationParameters TokenValidationParameters { get; set; }

        public AuthController(
            ApiServerLibraryTestConfig config, 
            UserManager<TestUser> userManager,
            ApiServerLibraryTestDbContext context,
            TokenValidationParameters tokenValidationParameters)
        {
            Config = config;
            UserManager = userManager;
            RefreshTokenManager = new RefreshTokenManager<RefreshToken<TestUser, int>, int>(new RefreshTokenStore<TestUser, int>(context));
            TokenValidationParameters = tokenValidationParameters;
            AuthorizationCodeManager = new AuthorizationCodeManager<AuthorizationCode<TestUser, int>, int>(new AuthorizationCodeStore<TestUser, int>(context));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            var res = await AuthenticationTools.LoginAsync<TestUser, int, RefreshToken<TestUser, int>, TokenResponse>(
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
            var res = await AuthenticationTools.VerifyAndGenerateTokenAsync<TestUser, int, RefreshToken<TestUser, int>, TokenResponse>(
                refreshTokenRequest: refreshTokenRequest,
                config: Config,
                refreshTokenManager: RefreshTokenManager,
                userManager: UserManager,
                tokenValidationParameters: TokenValidationParameters);

            return Ok(res);
        }

        [HttpPost("exchange")]
        public async Task<IActionResult> ExchangeAuthCodeAsync([FromBody] ExchangeAuthorizationCodeRequest exchangeAuthorizationCodeRequest)
        {
            var res = await AuthorizationCodeManager.ValidateAsync(
                code: exchangeAuthorizationCodeRequest.AuthorizationCode,
                codeVerifier: exchangeAuthorizationCodeRequest.CodeVerifier);

            if (!res.Success)
                return this.DiscernErrorActionResult(res);

            AuthorizationCode<TestUser, int> authorizationCode = res.Data;

            TestUser user = await UserManager.FindByIdAsync(authorizationCode.UserId.ToString());


            var tokenResponse = await AuthenticationTools.GenerateTokenResponse<TestUser, int, RefreshToken<TestUser, int>, TokenResponse>(
                config: Config,
                user: user,
                roles: (await UserManager.GetRolesAsync(user)).ToList(),
                refreshTokenManager: RefreshTokenManager,
                addAdditionalClaims: (user) =>
                {
                    return new List<Claim>
                    {
                        new Claim(OAuthClaimTypes.ApplicationId, authorizationCode.AppId.ToString())
                    };
                });

            return Ok(tokenResponse);
        }
    }
}
