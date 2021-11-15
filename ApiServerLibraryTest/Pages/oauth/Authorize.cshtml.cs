using ApiServerLibraryTest.Data;
using FBase.ApiServer.EntityFramework;
using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiServerLibraryTest.Pages.oauth
{
    public class AuthorizeModel : PageModel
    {
        private CredentialSetManager<CredentialSet<TestUser, int>, int> CredentialSetManager { get; set; }
        private AppManager<App<TestUser, int>, int>? AppManager { get; set; }
        private AppAuthorizationManager<AppAuthorization<TestUser, int>, int> AppAuthorizationManager { get; set; }
        private AuthorizationCodeManager<AuthorizationCode<TestUser, int>, int> AuthorizationCodeManager { get; set; }
        private UserManager<TestUser> UserManager { get; set; }
        
        public string? AppName { get; set; }
        public List<string>? Scopes { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ClientId { get; set; }
        public string? CodeChallenge { get; set; }
        public string? RedirectUrl { get; set; }
        public long? AppId { get; set; }
        public int? UserId { get; set; }
        public bool UserHasAlreadyAuthorizedApp { get; set; } = false;

        public AuthorizeModel(ApiServerLibraryTestDbContext context, UserManager<TestUser> userManager)
        {
            CredentialSetManager = new CredentialSetManager<CredentialSet<TestUser, int>, int>(new CredentialSetStore<TestUser, int>(context));
            AppManager = new AppManager<App<TestUser, int>, int>(new AppStore<TestUser, int>(context));
            AppAuthorizationManager = new AppAuthorizationManager<AppAuthorization<TestUser, int>, int>(new AppAuthorizationStore<TestUser, int>(context));
            AuthorizationCodeManager = new AuthorizationCodeManager<AuthorizationCode<TestUser, int>, int>(new AuthorizationCodeStore<TestUser, int>(context));
            UserManager = userManager;
        }

        private async Task<IActionResult> ProcessAndRedirectWithAuthCode(int userId, string codeChallenge, long appId, string redirectUrl)
        {
            var createRes = await AuthorizationCodeManager!.CreateAsync(codeChallenge, userId, appId);

            if (!createRes.Success)
            {
                ErrorMessage = "Authorization Code creation failed";
                return Page();
            }

            var authorizationCode = createRes.Data;

            return Redirect($"{redirectUrl}?code={authorizationCode.Code}");
        }

        public async Task<IActionResult> OnGet(
            [FromQuery(Name = "client_id")] string clientId = "",
            [FromQuery(Name = "code_challenge")] string codeChallenge = "")
        {
            var credentialSet = await CredentialSetManager!.FindByClientIdAsync(clientId);

            if (credentialSet == null)
            {
                ErrorMessage = "Client Not Found";
                return Page();
            }
            else
            {
                ClientId = clientId;
                CodeChallenge = codeChallenge;
                RedirectUrl = credentialSet.RedirectUrl;
                var app = await AppManager!.FindByIdAsync(credentialSet.AppId);
                AppName = app.Name;

                if (User.Identity!.IsAuthenticated)
                {
                    var user = await UserManager!.FindByNameAsync(User.Identity!.Name);

                    var appAuthorization = await AppAuthorizationManager!.GetAppAuthorizationAsync(app.Id, user.Id);

                    if (appAuthorization != null)
                    {
                        UserHasAlreadyAuthorizedApp = true;

                        // return Redirect($"{RedirectUrl}?code=ABCDEFG");
                        return await ProcessAndRedirectWithAuthCode(user.Id, CodeChallenge, app.Id, credentialSet.RedirectUrl!);
                    }
                    else
                    {
                        Scopes = AppManager.GetScopes(app.Id);
                        UserId = user.Id;
                        AppId = app.Id;
                        return Page();
                    }
                }
                else // not authenticated
                {
                    return Page();
                }
                
            }
        }

        public IActionResult OnPostGoToLogIn(string clientId, string codeChallenge)
        {
            return Redirect($"/Login?client_id={clientId}&code_challenge={codeChallenge}");
        }

        public async Task<IActionResult> OnPostAuthorize(int userId, long appId, string redirectUrl, string codeChallenge)
        {
            var res = await AppAuthorizationManager!.AuthorizeAsync(appId, userId);

            if (!res.Success)
            {
                ErrorMessage = "Something went wrong with authorization";
                return Page();
            }

            // Create the authorization code later
            return await ProcessAndRedirectWithAuthCode(userId, codeChallenge, appId, redirectUrl);
        }

    }
}
