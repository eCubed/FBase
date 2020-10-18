using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FBase.ApiServer
{
    public class ApiKeyMiddleware<TApiClient, TKey>
        where TApiClient : class, IApiClient<TKey>
    {
        private RequestDelegate _next;
        private ApiKeyMiddlewareOptions ApiKeyMiddlewareOptions;

        public ApiKeyMiddleware(RequestDelegate next, ApiKeyMiddlewareOptions apiKeyMiddlewareOptions = null)
        {
            _next = next;
            ApiKeyMiddlewareOptions = apiKeyMiddlewareOptions ?? new ApiKeyMiddlewareOptions();
        }

        private ClaimsPrincipal CreateClaimsPrincipalForClient(TKey clientId)
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ApiKeyMiddlewareOptions.ClientIdentifierKey, clientId.ToString() ));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);

            principal.AddIdentity(claimsIdentity);

            return principal;
        }

        public async Task Invoke(HttpContext context, IApiClientProvider<TApiClient, TKey> apiClientProvider, IApiClientHasher apiClientHasher)
        {
            if (context.Request.Headers.ContainsKey(ApiKeyMiddlewareOptions.ApiKeyHeaderKey))
            {
                if (!context.Request.Headers.ContainsKey(ApiKeyMiddlewareOptions.HashHeaderKey))
                {
                    await ApiServerMiddlewareHelpers.WriteJsonErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ApiServerMessages.HashHeaderRequired);
                    return;
                }

                if (!context.Request.Headers.ContainsKey(ApiKeyMiddlewareOptions.DataHeaderKey))
                {
                    await ApiServerMiddlewareHelpers.WriteJsonErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ApiServerMessages.DataHeaderRequired);
                    return;
                }

                string apiKey = context.Request.Headers[ApiKeyMiddlewareOptions.ApiKeyHeaderKey].ToString();

                TApiClient apiClient = apiClientProvider.GetClientByApiKey(apiKey);

                if (apiClient == null)
                {
                    await ApiServerMiddlewareHelpers.WriteJsonErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ApiServerMessages.InvalidClient);
                    return;
                }

                string[] clientAuthValues = context.Request.Headers[ApiKeyMiddlewareOptions.HashHeaderKey].ToString().Split(' ');

                if (clientAuthValues.Length != 1)
                {
                    await ApiServerMiddlewareHelpers.WriteJsonErrorResponseAsync(context, StatusCodes.Status400BadRequest, ApiServerMessages.HashHeaderRequired);
                    return;
                }

                string clientHash = clientAuthValues[0];
                string clientSecret = apiClient.Secret;

                string xInputValue = context.Request.Headers[ApiKeyMiddlewareOptions.DataHeaderKey].ToString();

                if (!apiClientHasher.CheckHash(apiKey, clientSecret, xInputValue, clientHash))
                {
                    await ApiServerMiddlewareHelpers.WriteJsonErrorResponseAsync(context, StatusCodes.Status401Unauthorized, ApiServerMessages.InvalidHash);
                    return;
                }

                // Now that the headers validate, we now need to store that client id into the principal...
                context.User = CreateClaimsPrincipalForClient(apiClient.Id);
            }

            await _next.Invoke(context);
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware<TApiClient, TKey>(this IApplicationBuilder applicationBuilder,
            ApiKeyMiddlewareOptions options = null)
            where TApiClient: class, IApiClient<TKey>
        {
            if (options == null)
                options = new ApiKeyMiddlewareOptions();

            return applicationBuilder.UseMiddleware<ApiKeyMiddleware<TApiClient, TKey>>(options);
        }
    }
}
