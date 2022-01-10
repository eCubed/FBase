using FBase.Foundations;
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
}
