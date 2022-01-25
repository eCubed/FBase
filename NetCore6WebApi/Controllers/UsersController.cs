using FBase.Api;
using FBase.Api.Server;
using FBase.Api.Server.Controllers;
using FBase.Api.Server.Providers;
using FBase.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        TestingDbContext context,
        TestingConfig config,
        UserManager<TestingUser> userManager,
        ProgramSetupOptions<TestingConfig, TestingUser, string> programSetupOptions,
        ICrypter crypter,
        IUserAccountCorresponder<TestingUser, string> userAccountCorresponder
        ) : base(userManager, context, crypter, config, programSetupOptions, userAccountCorresponder)
    {
    }

    protected override IAppUserStore<TestingUser, string> InstantiateSpecificAppUserStore(DbContext db)
    {
        return new TestingAppUserStore((TestingDbContext)db);
    }
}
