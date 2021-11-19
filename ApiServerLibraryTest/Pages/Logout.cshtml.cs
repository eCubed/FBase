using ApiServerLibraryTest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Pages
{
    public class LogoutModel : PageModel
    {
        private SignInManager<TestUser> SignInManager { get; set; }

        public LogoutModel(SignInManager<TestUser> signInManager)
        {
            SignInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if(User.Identity!.IsAuthenticated)
            {
                await SignInManager.SignOutAsync();
                return Redirect("/");
            }

            return Page();
        }
    }
}
