using FBase.Api.Server.Models;
using FBase.Foundations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FBase.Api.Server.Utils;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, ManagerResult res, HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        if (res.Errors == null || !res.Errors.Any())
        {
            switch (successStatusCode)
            {
                case HttpStatusCode.Created: return controller.StatusCode(201);
                case HttpStatusCode.NoContent: return controller.NoContent();
                default: return controller.Ok();
            }
        }

        List<string> errors = res.Errors.ToList();

        if (errors.Contains(ManagerErrors.Unauthorized))
            return controller.StatusCode(401, new { Errors = errors });
        else
            return controller.BadRequest(new { Errors = errors });
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, ManagerResult res, T? objectToReturn = default(T), HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        if (res.Errors == null || !res.Errors.Any())
        {
            switch(successStatusCode)
            {
                case HttpStatusCode.Created: return (objectToReturn == null) ? controller.StatusCode(201) : controller.StatusCode(201, objectToReturn);
                case HttpStatusCode.NoContent: return controller.NoContent();
                default: return (objectToReturn == null) ? controller.Ok() : controller.Ok(objectToReturn);
            }
        }

        List<string> errors = res.Errors.ToList();

        if (errors.Contains(ManagerErrors.Unauthorized))
            return controller.StatusCode(401, new { Errors = errors });
        else
            return controller.BadRequest(new { Errors = errors });
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, ManagerResult<T> res, HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        if (res.Errors == null || !res.Errors.Any())
        {
            switch (successStatusCode)
            {
                case HttpStatusCode.Created: return controller.StatusCode(201, res.Data);
                case HttpStatusCode.NoContent: return controller.NoContent();
                default: return controller.Ok(res.Data);
            }
        }

        List<string> errors = res.Errors.ToList();

        if (errors.Contains(ManagerErrors.Unauthorized))
            return controller.StatusCode(401, new { Errors = errors });
        else
            return controller.BadRequest(new { Errors = errors });
    }

    public static async Task<AuthenticatedInfo<TUserKey>> ResolveAuthenticatedEntitiesAsync<TUser, TUserKey>(
        this ControllerBase controller,
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
        this ControllerBase controller, 
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
