using FBase.Foundations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FBase.ApiServer
{
    public static class ApiServerMiddlewareHelpers
    {
        public static async Task WriteJsonErrorResponseAsync(HttpContext context, int statusCode, string errorMessage, Exception e = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json;charset=utf-8";
            var response = (e != null) ? e.CreateManagerResult(errorMessage).Errors.ToList() :
                                         new List<string> { errorMessage };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            }));
        }
    }
}
