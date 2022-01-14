using FBase.Api.Server;
using FBase.Api.Server.Controllers;
using FBase.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetCore6WebApi.Data;
using NetCore6WebApi.Models;

// <TApiServerConfig, TDbContext, TUser, TRole, TUserKey, TRegisterModel> 

namespace NetCore6WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : UsersBaseController<TestingConfig, TestingUser, string, RegisterModel>
{
    public UsersController(
        TestingConfig config,
        TestingDbContext context,
        UserManager<TestingUser> userManager,
        TokenValidationParameters tokenValidationParameters,
        ProgramSetupOptions<TestingConfig, TestingUser, string> programSetupOptions,
        ICrypter crypter
        ) : base(userManager, crypter, config, programSetupOptions)
    {
    }
}
