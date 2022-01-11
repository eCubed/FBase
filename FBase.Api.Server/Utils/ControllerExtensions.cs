using FBase.Api.Server.Models;
using FBase.Foundations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBase.Api.Server.Utils;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ManagerResult res, T? objectToReturn = default(T))
    {
        if (res.Errors == null || !res.Errors.Any())
            return (objectToReturn == null) ? controller.Ok() : controller.Ok(objectToReturn);

        List<string> errors = res.Errors.ToList();

        if (errors.Contains(ManagerErrors.Unauthorized))
            return controller.StatusCode(401, new { Errors = errors });
        else
            return controller.BadRequest(new { Errors = errors });
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, ManagerResult<T> res)
    {
        if (res.Errors == null || !res.Errors.Any())
            return controller.Ok(res.Data);

        List<string> errors = res.Errors.ToList();

        if (errors.Contains(ManagerErrors.Unauthorized))
            return controller.StatusCode(401, new { Errors = errors });
        else
            return controller.BadRequest(new { Errors = errors });
    }

    public static async Task<AuthenticatedInfo<TUserKey>> ResolveAuthenticatedEntitiesAsync<TUser, TUserKey>(
        this Controller controller,
        UserManager<TUser> userManager)
        where TUser: IdentityUser<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        string username = controller.Request.HttpContext.User?.Identity?.Name ?? "";
        TUser user = await userManager.FindByNameAsync(username);

        TUserKey? userId = (user == null) ? default(TUserKey) : user.Id;

        return new AuthenticatedInfo<TUserKey>
        {
            UserId = userId
        };
    }

    public static async Task<bool> IsRequestorAdminAsync<TUser, TUserKey>(
        this Controller controller, 
        UserManager<TUser> userManager,
        string administratorRoleName)
        where TUser : IdentityUser<TUserKey>
        where TUserKey : IEquatable<TUserKey>
    {
        string username = controller.Request.HttpContext.User?.Identity?.Name ?? "";
        TUser user = await userManager.FindByNameAsync(username);

        return (user == null) ? false : (await userManager.IsInRoleAsync(user, administratorRoleName));
    }
}
