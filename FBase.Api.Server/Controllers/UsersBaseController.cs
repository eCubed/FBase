using FBase.Api.EntityFramework;
using FBase.Api.Server.Models;
using FBase.Api.Server.Utils;
using FBase.Cryptography;
using FBase.Foundations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FBase.Api.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class UsersBaseController<TApiServerConfig, TUser, TUserKey, TRegisterModel> : ControllerBase
    where TUser : IdentityUser<TUserKey>, new()
    where TUserKey : IEquatable<TUserKey>
    where TApiServerConfig : class, IApiServerConfig, new()
    where TRegisterModel: class, IRegisterModel
{
    private UserManager<TUser> UserManager { get; set; }
    private ICrypter Crypter { get; set; }
    private TApiServerConfig Config { get; set; }
    private ProgramSetupOptions<TApiServerConfig, TUser, TUserKey> ProgramSetupOptions { get; set; }

    public UsersBaseController(
        UserManager<TUser> userManager, 
        ICrypter crypter,
        TApiServerConfig config,
        ProgramSetupOptions<TApiServerConfig, TUser, TUserKey> programSetupOptions)
    {
        UserManager = userManager;
        Crypter = crypter;
        Config = config;
        ProgramSetupOptions = programSetupOptions;
        
    }

    /*
    [HttpGet("search/forselection/{searchText}/{pg}/{pageSize}")]
    public async Task<IActionResult> SearchUsersForSelectionAsync(string searchText, int pg = 1, int pageSize = 10)
    {
        if (!await this.IsRequestorAdminAsync<TUser, TUserKey>(UserManager, Config.AdminRoleName)) {
            return Unauthorized();
        }

        var searchRes = UserManager.SearchUsers(
            searchText: searchText,
            page: pg,
            pageSize: pageSize,
            projector: (user) =>
            {
                return new UserForSelectionModel { Id = user.Id, Email = user.Email };
            });

        return Ok(searchRes);
    }
    */

    /*
    [HttpGet("search/{searchText}/{pg}/{pageSize}")]
    [Authorize(Roles = "Administrators")]
    public IActionResult SearchUsersAsync(string searchText, int pg = 1, int pageSize = 10)
    {
        var searchRes = TUserManager.SearchUsers(
            searchText: searchText,
            page: pg,
            pageSize: pageSize,
            projector: (user) =>
            {
                return new UserForAdminListItem
                {
                    Id = user.Id,
                    Email = user.Email,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsUserLockedOut = UserManager.IsLockedOutAsync(user).Result
                };
            });

        return Ok(searchRes);
    }
    */

    [HttpPut("{id}/confirm/email")]
    // [Authorize(Roles = "Administrators")] /* Commented out because this abstraction cannot dynamically set attribute values */
    public async Task<IActionResult> ConfirmEmailAsync(long id)
    {
        if (!await this.IsRequestorAdminAsync<TUser, TUserKey>(UserManager, ProgramSetupOptions.AdminRoleName))
        {
            return Unauthorized();
        }

        TUser user = await UserManager.FindByIdAsync(id.ToString());

        if (user == null)
            return this.ToActionResult(new ManagerResult<TUser>(ManagerErrors.RecordNotFound));

        if (ProgramSetupOptions.UserAccountCorresponder != null)
        {
            string token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmRes = await UserManager.ConfirmEmailAsync(user, token);

            if (!confirmRes.Succeeded)
                return this.ToActionResult(new ManagerResult<TUser>(confirmRes.Errors.Select(e => e.Description).ToList()));

            ProgramSetupOptions.UserAccountCorresponder.SendEmailConfirmedCorrespondence(user);
        }

        return Ok();
    }

    [HttpPut("{id}/lockout")]
    // [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> LockoutUserAsync(long id)
    {
        if (!await this.IsRequestorAdminAsync<TUser, TUserKey>(UserManager, ProgramSetupOptions.AdminRoleName))
        {
            return Unauthorized();
        }

        TUser user = await UserManager.FindByIdAsync(id.ToString());

        if (user == null)
            return this.ToActionResult(new ManagerResult<TUser>(ManagerErrors.RecordNotFound));

        var lockoutRes = await UserManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(200));

        if (!lockoutRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(lockoutRes.Errors.Select(e => e.Description).ToList()));

        ProgramSetupOptions.UserAccountCorresponder?.SendAccountLockedOutCorrespondence(user);

        return Ok(new { LockoutEndDate = DateTime.Now.AddYears(200) });
    }

    [HttpPut("{id}/endlockout")]
    // [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> EndLockoutUserAsync(long id)
    {
        if (!await this.IsRequestorAdminAsync<TUser, TUserKey>(UserManager, ProgramSetupOptions.AdminRoleName))
        {
            return Unauthorized();
        }

        TUser user = await UserManager.FindByIdAsync(id.ToString());

        if (user == null)
            return this.ToActionResult(new ManagerResult<TUser>(ManagerErrors.RecordNotFound));

        var lockoutRes = await UserManager.SetLockoutEndDateAsync(user, DateTime.Now.AddDays(-1));

        if (!lockoutRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(lockoutRes.Errors.Select(e => e.Description).ToList()));

        ProgramSetupOptions.UserAccountCorresponder?.SendAccountReinstatedCorrespondence(user);

        return Ok();
    }

    [HttpGet("check/istaken/{email}")]
    public async Task<IActionResult> CheckIsEmailTakenAsync(string email)
    {
        TUser user = await UserManager.FindByEmailAsync(email);

        return Ok(new { IsEmailTaken = (user != null) });
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] TRegisterModel registerModel)
    {
        TUser user = new TUser();
        user.Email = registerModel.Email;
        user.EmailConfirmed = false;
        user.UserName = registerModel.Email;
        // What about extra fields!? We'll need a helper provider for that!

        var createRes = await UserManager.CreateAsync(user, registerModel.Password);

        if (!createRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(createRes.Errors.Select(e => e.Description).ToList()));

        var addRoleRes = await UserManager.AddToRoleAsync(user, ProgramSetupOptions.RegularUserRoleName);

        if (!addRoleRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(createRes.Errors.Select(e => e.Description).ToList()));

        if (ProgramSetupOptions.UserAccountCorresponder != null)
        {
            string emailConfirmationToken = WebUtility.UrlEncode(await UserManager.GenerateEmailConfirmationTokenAsync(user)).Replace("%2B", "%20");

            string encryptedEmail = WebUtility.UrlEncode(Crypter.Encrypt(user.Email, Config.CryptionKey)).Replace("%2B", "%20");

            ProgramSetupOptions.UserAccountCorresponder.SendSuccessfulRegistrationCorrespondence(user, emailConfirmationToken, encryptedEmail);
        }

        return StatusCode(201);
    }

    [HttpPost("email/confirm")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailModel confirmEmailModel)
    {
        try
        {
            string email = Crypter.Decrypt(confirmEmailModel.EncryptedEmail.Replace(" ", "+"), Config.CryptionKey);
            TUser user = await UserManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound(new ManagerResult(ManagerErrors.RecordNotFound));

            var res = await UserManager.ConfirmEmailAsync(user,
                token: confirmEmailModel.EmailConfirmationToken.Replace(" ", "+"));

            if (!res.Succeeded)
                return this.ToActionResult<TUser>(new ManagerResult(res.Errors.Select(er => er.Description).ToArray()));

            ProgramSetupOptions.UserAccountCorresponder?.SendEmailConfirmedCorrespondence(user);

            return Ok();
        }
        catch (Exception e)
        {
            return this.ToActionResult<TUser>(e.CreateManagerResult());
        }
    }

    [HttpPost("request/password/reset")]
    public async Task<IActionResult> RequestPasswordResetAsync([FromBody] RequestPasswordResetModel requestPasswordResetModel)
    {
        TUser user = await UserManager.FindByEmailAsync(requestPasswordResetModel.Email);

        if (user == null)
            return NotFound(new ManagerResult(ManagerErrors.RecordNotFound));

        string passwordResetToken = WebUtility.UrlEncode(await UserManager.GeneratePasswordResetTokenAsync(user)).Replace("%2B", "%20");

        string encryptedEmail = WebUtility.UrlEncode(Crypter.Encrypt(user.Email, Config.CryptionKey)).Replace("%2B", "%20");

        ProgramSetupOptions.UserAccountCorresponder?.SendPasswordResetRequestCorrespondence(user, passwordResetToken, encryptedEmail);

        return Ok();
    }

    [HttpPost("password/reset")]
    public async Task<IActionResult> PasswordReset([FromBody] PasswordResetModel passwordResetModel)
    {
        try
        {
            string email = Crypter.Decrypt(passwordResetModel.EncryptedEmail.Replace(" ", "+"), Config.CryptionKey);
            TUser user = await UserManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound(new ManagerResult(ManagerErrors.RecordNotFound));

            var res = await UserManager.ResetPasswordAsync(user,
                token: passwordResetModel.PasswordResetToken.Replace(" ", "+"),
                passwordResetModel.NewPassword.Replace(" ", "+"));

            if (!res.Succeeded)
                return this.ToActionResult<TUser>(new ManagerResult(res.Errors.Select(er => er.Description).ToArray()));

            return Ok();
        }
        catch (Exception e)
        {
            return this.ToActionResult<TUser>(e.CreateManagerResult());
        }
    }
}
