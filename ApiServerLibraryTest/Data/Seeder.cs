using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class Seeder
    {
        private ApiServerLibraryTestDbContext db { get; set; }
        private UserManager<TestUser> UM { get; set; }
        private RoleManager<TestRole> RM { get; set; }

        private ScopeManager<Scope> ScopeManager { get; set; }
        private AppManager<App, int> AppManager { get; set; }
        private CredentialSetManager<CredentialSet> CredentialSetManager { get; set; }

        public Seeder(ApiServerLibraryTestDbContext context, UserManager<TestUser> userManager,
           RoleManager<TestRole> roleManager)
        {
            db = context;
            UM = userManager;
            RM = roleManager;
            ScopeManager = new ScopeManager<Scope>(new ScopeStore(context));
            AppManager = new AppManager<App, int>(new AppStore(context));
            CredentialSetManager = new CredentialSetManager<CredentialSet>(new CredentialSetStore(context));
        }

        public async Task CreateRolesAsync()
        {
            if (!(await RM.RoleExistsAsync(RoleNames.Administrator)))
                await RM.CreateAsync(new TestRole { Name = RoleNames.Administrator });

            if (!(await RM.RoleExistsAsync(RoleNames.Subscriber)))
                await RM.CreateAsync(new TestRole { Name = RoleNames.Subscriber });
        }

        private async Task CreateUserAsync(string username, List<string> roleNames)
        {
            TestUser user = await UM.FindByNameAsync(username);

            if (user == null)
            {
                string usernameToSave = $"{username}@test123123.com";
                user = new TestUser
                {
                    UserName = usernameToSave,
                    Email = usernameToSave,
                    EmailConfirmed = true
                };

                var res = await UM.CreateAsync(user, "Aaa000$");

                if (res.Succeeded)
                {
                    roleNames.ForEach(roleName =>
                    {
                        var addRoleRes = UM.AddToRoleAsync(user, roleName).Result;
                    });

                }
            }
        }

        public async Task CreateUsersAsync()
        {
            await CreateUserAsync("admin", new List<string> { RoleNames.Administrator, RoleNames.Subscriber });
            await CreateUserAsync("user1", new List<string> { RoleNames.Subscriber });
            await CreateUserAsync("user2", new List<string> { RoleNames.Subscriber });
            await CreateUserAsync("user3", new List<string> { RoleNames.Subscriber });
        }

        public async Task CreateScopeAsync(string name, string description)
        {
            Scope scope = await ScopeManager.FindByNameAsync(name);

            if (scope == null)
            {
                await ScopeManager.CreateAsync(name, description);
            }
        }

        public async Task CreateScopesAsync()
        {
            await CreateScopeAsync("http://apiserverlibrarytest/scope/email", "Scope to get the email address of a user");
        }

        public async Task CreateAppAsync(string name)
        {
            App app = await AppManager.FindByNameAsync(name);

            if (app == null)
            {
                TestUser user = await UM.FindByNameAsync("user1@test123123.com");

                var createRes = await AppManager.CreateAsync(name, user.Id);

                if (createRes.Success)
                {
                    // Create Scopes
                    await AppManager.AddScopeToAppAsync("http://apiserverlibrarytest/scope/email", createRes.Data.Id);


                    // Create The Credentials
                    await CredentialSetManager.CreateAsync("Native Client", createRes.Data.Id);
                }
            }
        }

        public async Task InitializeDataAsync()
        {
            await db.Database.EnsureCreatedAsync();
            await CreateRolesAsync();
            await CreateUsersAsync();
            await CreateScopesAsync();
            await CreateAppAsync("TNT Hall of Fame");
        }
    }
}
