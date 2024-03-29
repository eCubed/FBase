﻿using FBase.Api.EntityFramework;
using FBase.Api.Server.Models;
using FBase.Api.Server.Providers;
using FBase.Api.Server.Utils;
using FBase.Cryptography;
using FBase.Foundations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FBase.Api.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class UsersBaseController<TApiServerConfig, TUser, TUserKey, TRegisterModel> : ControllerBase
    where TUser : IdentityUser<TUserKey>, IAppUser<TUserKey>, new()
    where TUserKey : IEquatable<TUserKey>
    where TApiServerConfig : class, IApiServerConfig, new()
    where TRegisterModel: class, IRegisterModel
{
    private UserManager<TUser> UserManager { get; set; }
    private AppUserManager<TUser, TUserKey> AppUserManager { get; set; }
    private ICrypter Crypter { get; set; }
    private TApiServerConfig Config { get; set; }
    private ProgramSetupOptions<TApiServerConfig, TUser, TUserKey> ProgramSetupOptions { get; set; }
    private IUserAccountCorresponder<TUser, TUserKey> UserAccountCorresponder { get; set; }
    private DbContext db { get; set; }

    public UsersBaseController(
        UserManager<TUser> userManager,
        DbContext context,
        ICrypter crypter,
        TApiServerConfig config,
        ProgramSetupOptions<TApiServerConfig, TUser, TUserKey> programSetupOptions,
        IUserAccountCorresponder<TUser, TUserKey> userAccountCorresponder)
    {

        db = context;
        UserManager = userManager;
        AppUserManager = new AppUserManager<TUser, TUserKey>(InstantiateSpecificAppUserStore(db));
        Crypter = crypter;
        Config = config;
        ProgramSetupOptions = programSetupOptions;
        UserAccountCorresponder = userAccountCorresponder;
    }

    protected abstract IAppUserStore<TUser, TUserKey> InstantiateSpecificAppUserStore(DbContext db);

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

    [HttpGet("search/{searchText}/{pg}/{pageSize}")]
    public async Task<IActionResult> SearchUsersAsync(string searchText, int pg = 1, int pageSize = 10)
    {
        if (!await this.IsRequestorAdminAsync<TUser, TUserKey>(UserManager, ProgramSetupOptions.AdminRoleName))
        {
            return Unauthorized();
        }

        var searchRes = AppUserManager.SearchUsers(
            searchText: searchText,
            page: pg,
            pageSize: pageSize,
            projector: (user) => new UserForAdminListItem<TUserKey>
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockedOut = UserManager.IsLockedOutAsync(user).Result
            });

        return Ok(searchRes);
    }

    [HttpPut("{id}/confirm/email")]
    // [Authorize(Roles = "Administrators")] /* Commented out because this abstraction cannot dynamically set attribute values */
    public async Task<IActionResult> ConfirmEmailAsync(TUserKey id)
    {
        if (!await this.IsRequestorAdminAsync<TUser, TUserKey>(UserManager, ProgramSetupOptions.AdminRoleName))
        {
            return Unauthorized();
        }

        TUser user = await UserManager.FindByIdAsync(id.ToString());

        if (user == null)
            return this.ToActionResult(new ManagerResult<TUser>(ManagerErrors.RecordNotFound));

        string token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmRes = await UserManager.ConfirmEmailAsync(user, token);

        if (!confirmRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(confirmRes.Errors.Select(e => e.Description).ToList()));

        UserAccountCorresponder.SendEmailConfirmedCorrespondence(user);

        return Ok();
    }

    [HttpPut("{id}/lockout")]
    // [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> LockoutUserAsync(TUserKey id)
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

        UserAccountCorresponder.SendAccountLockedOutCorrespondence(user);

        return Ok(new { LockoutEndDate = DateTime.Now.AddYears(200) });
    }

    [HttpPut("{id}/endlockout")]
    // [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> EndLockoutUserAsync(TUserKey id)
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

        UserAccountCorresponder.SendAccountReinstatedCorrespondence(user);

        return Ok();
    }

    [HttpGet("check/istaken/{email}")]
    public async Task<IActionResult> CheckIsEmailTakenAsync(string email)
    {
        TUser user = await UserManager.FindByEmailAsync(email);

        return Ok(new { IsEmailTaken = (user != null) });
    }

    protected virtual void FillCustomUserForRegistration(TUser user, TRegisterModel registerModel)
    {
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] TRegisterModel registerModel)
    {
        TUser user = new TUser();
        user.Email = registerModel.Email;
        user.EmailConfirmed = false;
        user.UserName = registerModel.Email;
        // What about extra fields!? We'll need a helper provider for that!
        FillCustomUserForRegistration(user, registerModel);

        var createRes = await UserManager.CreateAsync(user, registerModel.Password);

        if (!createRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(createRes.Errors.Select(e => e.Description).ToList()));

        var addRoleRes = await UserManager.AddToRoleAsync(user, ProgramSetupOptions.RegularUserRoleName);

        if (!addRoleRes.Succeeded)
            return this.ToActionResult(new ManagerResult<TUser>(createRes.Errors.Select(e => e.Description).ToList()));

        string emailConfirmationToken = WebUtility.UrlEncode(await UserManager.GenerateEmailConfirmationTokenAsync(user)).Replace("%2B", "%20");

        string encryptedEmail = WebUtility.UrlEncode(Crypter.Encrypt(user.Email, Config.CryptionKey)).Replace("%2B", "%20");

        UserAccountCorresponder.SendSuccessfulRegistrationCorrespondence(user, emailConfirmationToken, encryptedEmail);
        
        return StatusCode(201);
    }

    [HttpPost("email/confirm")]
    public async Task<IActionResult> ConfirmEmailAsync([FromBody] ConfirmEmailModel confirmEmailModel)
    {
        try
        {
            string email = Crypter.Decrypt(WebUtility.UrlDecode(confirmEmailModel.EncryptedEmail.Replace(" ", "+")), Config.CryptionKey);
            TUser user = await UserManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound(new ManagerResult(ManagerErrors.RecordNotFound));

            string decodedToken = WebUtility.UrlDecode(confirmEmailModel.EmailConfirmationToken).Replace(" ", "+");

            var res = await UserManager.ConfirmEmailAsync(user,
                token: decodedToken);

            if (!res.Succeeded)
                return this.ToActionResult<TUser>(new ManagerResult(res.Errors.Select(er => er.Description).ToArray()));

            UserAccountCorresponder.SendEmailConfirmedCorrespondence(user);

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

        UserAccountCorresponder.SendPasswordResetRequestCorrespondence(user, passwordResetToken, encryptedEmail);

        return Ok();
    }

    [HttpPost("password/reset")]
    public async Task<IActionResult> PasswordReset([FromBody] PasswordResetModel passwordResetModel)
    {
        try
        {
            string email = Crypter.Decrypt(WebUtility.UrlDecode(passwordResetModel.EncryptedEmail.Replace(" ", "+")), Config.CryptionKey);
            TUser user = await UserManager.FindByEmailAsync(email);

            if (user == null)
                return NotFound(new ManagerResult(ManagerErrors.RecordNotFound));

            string decodedToken = WebUtility.UrlDecode(passwordResetModel.PasswordResetToken).Replace(" ", "+");

            var res = await UserManager.ResetPasswordAsync(user,
                token: decodedToken,
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

    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody]ChangePasswordModel changePasswordModel)
    {
        AuthenticatedInfo<TUserKey> authenticatedInfo = await this.ResolveAuthenticatedEntitiesAsync<TUser, TUserKey>(UserManager);

        if ((authenticatedInfo == null) || (authenticatedInfo.UserId == null))
            return Unauthorized();

        TUser user = await UserManager.FindByIdAsync(authenticatedInfo.UserId!.ToString());

        if (user == null)
            return Unauthorized();

        var res = await UserManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);

        if (!res.Succeeded)
        {
            return this.ToActionResult<TUser>(new ManagerResult<TUser>(res.Errors.Select(e => e.Description).ToList()));
        }

        return Ok();
    }
}
