using FBase.Api.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace FBase.Api.Server
{
    public class ProgramSetupOptions<TApiServerConfig, TUser, TUserKey>
        where TApiServerConfig : class, IApiServerConfig
        where TUser : IdentityUser<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        public string ConfigurationKey { get; set; } = "ConfigurationKey";
        public List<string> Roles { get; set; } = new List<string>();
        public string AdminRoleName { get; set; } = "administrator";
        public string RegularUserRoleName { get; set; } = "subscriber";
        public List<NewUserWithRoles> UserWithRoles { get; set; } = new List<NewUserWithRoles>();
        public Action<TApiServerConfig, WebApplicationBuilder>? SetupAdditionalEntities { get; set; } = null;
        public Action<TApiServerConfig, WebApplicationBuilder>? RegisterAdditionalServices { get; set; } = null;
        public Func<TApiServerConfig, StaticFileOptions>? ProvideStaticFileOptions { get; set; } = null;
        public IUserAccountCorresponder<TUser, TUserKey>? UserAccountCorresponder { get; set; } = null;
    }
}
