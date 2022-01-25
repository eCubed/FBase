using FBase.Api;
using Microsoft.AspNetCore.Identity;

namespace NetCore6WebApi.Data
{
    public class TestingUser : IdentityUser, IAppUser<string>
    {
    }
}
