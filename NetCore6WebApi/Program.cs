
using FBase.Api.EntityFramework;
using FBase.Api.Server;
using NetCore6WebApi;
using NetCore6WebApi.Data;

ProgramSetup.Configure<TestingConfig, TestingDbContext, TestingUser, TestingRole, string, TestingSeeder>(
    args: args,
    options: new ProgramSetupOptions
    {
        ConfigurationKey = "TestingConfig",
        Roles = new List<string> { "admin", "subscriber" },
        UserWithRoles = new List<NewUserWithRoles>
        {
            new NewUserWithRoles { Username = "admin@testing.com", Roles = new List<string> { "admin", "subscriber "}},
            new NewUserWithRoles { Username = "user1@testing.com", Roles = new List<string> { "subscriber" }},
            new NewUserWithRoles { Username = "user2@testing.com", Roles = new List<string> { "subscriber" }},
        }
    });