using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiServerLibraryTest.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiServerLibraryTest.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        private SignInManager<TestUser> SignInManager { get; set; }
        private UserManager<TestUser> UserManager { get; set; }

        public LoginModel(SignInManager<TestUser> signInManager, UserManager<TestUser> userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TestUser user = await UserManager.FindByNameAsync(Username);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "User does not exist";
                return Page();
                
            } 
            else
            {
                if (!await UserManager.CheckPasswordAsync(user, Password))
                {
                    ViewData["ErrorMessage"] = "Incorrect Password";
                    return Page();
                }
                else
                {
                    await SignInManager.SignInAsync(user, false, CookieAuthenticationDefaults.AuthenticationScheme);

                    return Redirect("/");
                }
            }
        }
    }
}
