using FBase.Api.EntityFramework;
using FBase.Api.Server.Models;
using FBase.Api.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FBase.Api.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class AuthBaseController<TApiServerConfig, TDbContext, TUser, TRole, TUserKey> : ControllerBase
    where TDbContext : ApiServerDbContext<TUser, TRole, TUserKey>
    where TUser : IdentityUser<TUserKey>, new()
    where TRole : IdentityRole<TUserKey>
    where TUserKey: IEquatable<TUserKey>
    where TApiServerConfig : class, IApiServerConfig, new()
{
    private TApiServerConfig Config { get; set; }
    private UserManager<TUser> UserManager { get; set; }
    private RefreshTokenManager<RefreshToken<TUser, TUserKey>, TUserKey> RefreshTokenManager { get; set; }
    private TokenValidationParameters TokenValidationParameters { get; set; }

    public AuthBaseController(TDbContext db, TApiServerConfig config, UserManager<TUser> userManager, TokenValidationParameters tokenValidationParameters)
    {
        Config = config;
        UserManager = userManager;
        RefreshTokenManager = new RefreshTokenManager<RefreshToken<TUser, TUserKey>, TUserKey>(new RefreshTokenStore<TUser, TUserKey>(db));
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
