
using FBase.Api.EntityFramework;
using FBase.Api.Server;
using NetCore6WebApi;
using NetCore6WebApi.Data;
using NetCore6WebApi.Providers;

ProgramSetup.Configure<TestingConfig, TestingDbContext, TestingUser, TestingRole, string, TestingSeeder, UserAccountCorresponder>(
    args: args,
    options: new ProgramSetupOptions<TestingConfig, TestingUser, string>
    {
        ConfigurationKey = "TestingConfig",
        Roles = new List<string> { "admin", "subscriber" },
        RegisterAdditionalServices = (config, app) =>
        {
            app.Services.AddTransient<IEmailer, DefaultEmailer>();
        },
        UserWithRoles = new List<NewUserWithRoles>
        {
            new NewUserWithRoles { Username = "admin@testing.com", Roles = new List<string> { "admin", "subscriber "}},
            new NewUserWithRoles { Username = "user1@testing.com", Roles = new List<string> { "subscriber" }},
            new NewUserWithRoles { Username = "user2@testing.com", Roles = new List<string> { "subscriber" }},
        }        
    });