using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiServerLibraryTest.Data;
using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiServerLibraryTest.Pages.oauth
{
    public class AuthorizeModel : PageModel
    {
        private CredentialSetManager<CredentialSet>? CredentialSetManager { get; set; }
        private AppManager<App, int>? AppManager { get; set; }
        private AppAuthorizationManager<AppAuthorization, int>? AppAuthorizationManager { get; set; }
        private UserManager<TestUser>? UserManager { get; set; }
        
        public bool ClientIdFound { get; set; }
        public string? AppName { get; set; }
        public List<string>? Scopes { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ClientId { get; set; }
        public string? RedirectUrl { get; set; }
        public long? AppId { get; set; }
        public int? UserId { get; set; }
        public bool UserHasAlreadyAuthorizedApp { get; set; } = false;

        public AuthorizeModel(ApiServerLibraryTestDbContext context, UserManager<TestUser> userManager)
        {
            CredentialSetManager = new CredentialSetManager<CredentialSet>(new CredentialSetStore(context));
            AppManager = new AppManager<App, int>(new AppStore(context));
            AppAuthorizationManager = new AppAuthorizationManager<AppAuthorization, int>(new AppAuthorizationStore(context));
            UserManager = userManager;
        }

        public async Task<IActionResult> OnGet([FromQuery(Name = "client_id")] string clientId = "")
        {
            var credentialSet = await CredentialSetManager!.FindByClientIdAsync(clientId);

            if (credentialSet == null)
            {
                ClientIdFound = false;
                ErrorMessage = "Client Not Found";
                return Page();
            }
            else
            {
                ClientIdFound = true;
                ClientId = clientId;
                RedirectUrl = credentialSet.RedirectUrl;
                var app = await AppManager!.FindByIdAsync(credentialSet.AppId);

                if (User.Identity!.IsAuthenticated)
                {
                    var user = await UserManager!.FindByNameAsync(User.Identity!.Name);

                    var appAuthorization = await AppAuthorizationManager!.GetAppAuthorizationAsync(app.Id, user.Id);

                    if (appAuthorization != null)
                    {
                        UserHasAlreadyAuthorizedApp = true;
                        // Generate authorization code, then redirect with auth code and other query parameters
                        return Redirect($"{RedirectUrl}?code=ABCDEFG");
                    }
                    else
                    {
                        AppName = app.Name;
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

        public IActionResult OnPostGoToLogIn(string clientId)
        {
            return Redirect($"/Login?client_id={clientId}");
        }

        public async Task<IActionResult> OnPostAuthorize(int userId, long appId, string redirectUrl)
        {
            var res = await AppAuthorizationManager!.AuthorizeAsync(appId, userId);

            if (!res.Success)
            {
                ErrorMessage = "Something went wrong with authorization";
                return Page();
            }

            // Create the authorization code later
            return Redirect($"{redirectUrl}?code=ABCDEFG");
        }
    }
}
