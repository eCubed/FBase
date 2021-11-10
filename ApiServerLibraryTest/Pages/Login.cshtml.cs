using ApiServerLibraryTest.Data;
using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiServerLibraryTest.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string? Username { get; set; }
        [BindProperty]
        public string? Password { get; set; }
        [BindProperty]
        public string? ClientId { get; set; }
        public string? AppName { get; set; }
        public List<string>? Scopes { get; set; }

        private SignInManager<TestUser>? SignInManager { get; set; }
        private UserManager<TestUser>? UserManager { get; set; }
        private AppAuthorizationManager<AppAuthorization, int>? AppAuthorizationManager { get; set; }
        private AppManager<App, int>? AppManager { get; set; }
        private CredentialSetManager<CredentialSet>? CredentialSetManager { get; set; }

        public LoginModel(ApiServerLibraryTestDbContext context, SignInManager<TestUser> signInManager, UserManager<TestUser> userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            AppAuthorizationManager = new AppAuthorizationManager<AppAuthorization, int>(new AppAuthorizationStore(context));
            CredentialSetManager = new CredentialSetManager<CredentialSet>(new CredentialSetStore(context));
            AppManager = new AppManager<App, int>(new AppStore(context));
        }

        public async Task OnGet([FromQuery(Name = "client_id")] string clientId = "")
        {
            ClientId = clientId;
            if(!string.IsNullOrEmpty(clientId))
            {
                CredentialSet credentialSet = await CredentialSetManager!.FindByClientIdAsync(clientId);
                App app = await AppManager!.FindByIdAsync(credentialSet!.AppId);
                AppName = app.Name;
                Scopes = AppManager.GetScopes(credentialSet!.AppId);
            }
            

        }

        private async Task<IActionResult> ManageAuthorizationAsync(string clientId, TestUser user)
        {
            CredentialSet credentialSet = await CredentialSetManager!.FindByClientIdAsync(clientId);

            if (credentialSet == null)
            {
                ViewData["ErrorMessage"] = "Invalid Client";
                return Page();
            } else
            {
                App app = await AppManager!.FindByIdAsync(credentialSet.AppId); // cannot be null now

                AppAuthorization authorization = await AppAuthorizationManager!.GetAppAuthorizationAsync(app.Id, user.Id);

                if (authorization == null)
                {
                    var createRes = await AppAuthorizationManager.AuthorizeAsync(app.Id, user.Id);

                    if (!createRes.Success)
                    {
                        ViewData["ErrorMessage"] = "Something Went Wrong With Authorization. Try again Later.";
                        return Page();
                    }
                }

                // Still need to create the authorization code record!
                return Redirect($"{credentialSet.RedirectUrl}?code=ABCDEFG");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TestUser user = await UserManager!.FindByNameAsync(Username);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "User does not exist";
                return Page();
                
            } 
            else
            {
                if (!await UserManager!.CheckPasswordAsync(user, Password))
                {
                    ViewData["ErrorMessage"] = "Incorrect Password";
                    return Page();
                }
                else
                {
                    await SignInManager!.SignInAsync(user, false, CookieAuthenticationDefaults.AuthenticationScheme);

                    if (!string.IsNullOrEmpty(ClientId))
                        return await ManageAuthorizationAsync(ClientId, user);

                    return Redirect("/");
                }
            }
        }
    }
}
