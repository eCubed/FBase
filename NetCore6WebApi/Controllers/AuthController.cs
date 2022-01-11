using FBase.Api.Server.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetCore6WebApi.Data;

namespace NetCore6WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AuthBaseController<TestingConfig, TestingDbContext, TestingUser, TestingRole, string>
    {
        public AuthController(
            TestingConfig config,
            TestingDbContext context,
            UserManager<TestingUser> userManager,
            TokenValidationParameters tokenValidationParameters
            ) : base(context, config, userManager, tokenValidationParameters)
        {
        }
    }
}
