using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiServerLibraryTest.Data;
using FBase.ApiServer.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiServerLibraryTest.Pages.oauth
{
    public class AuthorizeModel : PageModel
    {
        private CredentialSetManager<CredentialSet> CredentialSetManager { get; set; }
        private AppManager<App, int> AppManager { get; set; }
        
        public bool ClientIdFound { get; set; }
        public string? AppName { get; set; }
        public List<string>? Scopes { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ClientId { get; set; }

        public AuthorizeModel(ApiServerLibraryTestDbContext context)
        {
            CredentialSetManager = new CredentialSetManager<CredentialSet>(new CredentialSetStore(context));
            AppManager = new AppManager<App, int>(new AppStore(context));
        }


        public async Task OnGet([FromQuery(Name = "client_id")] string clientId = "")
        {
            var credentialSet = await CredentialSetManager.FindByClientIdAsync(clientId);

            if (credentialSet == null)
            {
                ClientIdFound = false;
                ErrorMessage = "Client Not Found";
            }
            else
            {
                ClientIdFound = true;
                ClientId = clientId;
                var app = await AppManager.FindByIdAsync(credentialSet.AppId);
                // app can't be null!
                AppName = app.Name;
                Scopes = AppManager.GetScopes(app.Id);

            }

            var dummy = 3;
        }

        public void OnPostGoToLogIn(string clientId)
        {
            var dummy = 5;
        }
    }
}
