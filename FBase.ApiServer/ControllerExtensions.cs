using FBase.Foundations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FBase.ApiServer
{
    public static class ControllerExtensions
    {
        public static async Task<TAuthenticatedInfo> ResolveAuthenticatedInfoAsync<TAuthenticatedInfo, TKey, TUser>(this ControllerBase controller,
            UserManager<TUser> userManager)
            where TAuthenticatedInfo : class, IAuthenticatedInfo<TKey>, new()
            where TUser: IdentityUser<TKey>
            where TKey : struct, IEquatable<TKey>
        {
            string username = controller.Request.HttpContext.User?.Identity?.Name ?? "";
            TUser user = await userManager.FindByNameAsync(username);

            TKey userId = user?.Id ?? default;

            return new TAuthenticatedInfo
            {
                RequestorId = userId,
                RequestorName = user?.UserName ?? ""
            };
        }

        public static async Task<TAuthenticatedInfo> ResolveApiClientAuthenticatedInfo<TAuthenticatedInfo, TKey, TApiClient>(this ControllerBase controller,
            IApiClientProvider<TApiClient, TKey> apiClientProvider)
            where TAuthenticatedInfo : class, IAuthenticatedInfo<TKey>, new()
            where TApiClient : class, IApiClient<TKey>
        {
            string publicKey = controller.Request.HttpContext.User?.Claims.SingleOrDefault(c => c.Type == "abc")?.Value ?? "";
            TApiClient apiClient = apiClientProvider.GetClientByApiKey(publicKey);

            if (apiClient == null)
                return null;

            await Task.FromResult(0);

            return new TAuthenticatedInfo
            {
                RequestorId = apiClient.Id,
                RequestorName = apiClient.Name
            };
        }

        public static async Task<bool> IsRequestorAdminAsync<TUser, TKey>(this ControllerBase controller, 
            UserManager<TUser> userManager, string administratorRoleName = "administrator")
            where TUser : IdentityUser<TKey>
            where TKey : struct, IEquatable<TKey>
        {
            string username = controller.Request.HttpContext.User?.Identity?.Name ?? "";
            TUser user = await userManager.FindByNameAsync(username);

            return (user == null) ? false : (await userManager.IsInRoleAsync(user, administratorRoleName));
        }

        public static IActionResult DiscernErrorActionResult(this ControllerBase controller, ManagerResult res)
        {
            if (!res.Errors.Any())
                throw new NotSupportedException();

            List<string> errors = res.Errors.ToList();

            if (errors.Contains(ManagerErrors.Unauthorized))
                return controller.StatusCode(401, new { Errors = errors });
            else if (errors.Contains(ManagerErrors.RecordNotFound))
                return controller.StatusCode(404, new { Errors = errors });
            else
                return controller.BadRequest(new { Errors = errors });
        }
    }
}
