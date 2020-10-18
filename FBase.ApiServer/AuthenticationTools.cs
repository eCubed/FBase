using FBase.Foundations;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FBase.ApiServer
{
    public static class AuthenticationTools
    {
        public static TTokenResponse GenerateTokenResponse<TTokenResponse, TKey>(IAppConfig appConfig,
            string username, TKey userId, List<string> roles,
            List<KeyValuePair<string, string>> additionalInfo = null)
            where TTokenResponse : class, ITokenResponse, new()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, username));
            roles.ForEach(roleName =>
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            });
            claims.Add(new Claim("UserId", userId.ToString()));

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig.CryptionKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: appConfig.Issuer,
                audience: appConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: signinCredentials
            );

            TTokenResponse tokenResponse = new TTokenResponse();
            tokenResponse.Username = username;
            tokenResponse.Token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            tokenResponse.Roles = roles;
            
            if (additionalInfo != null && additionalInfo.Count > 0)
                tokenResponse.SetAdditionalProperties(additionalInfo);

            return tokenResponse;
        }

        public static async Task<ManagerResult<TTokenResponse>> LoginAsync<TUser, TKey, TTokenResponse>(
            LoginModel loginModel, UserManager<TUser> userManager, IAppConfig appConfig,
            List<KeyValuePair<string, string>> additionalInfo = null)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TTokenResponse : class, ITokenResponse, new()
        {
            if (loginModel == null)
                return new ManagerResult<TTokenResponse>(ManagerErrors.InvalidRequest);

            var user = await userManager.FindByNameAsync(loginModel.Username);

            if (!await userManager.CheckPasswordAsync(user, loginModel.Password))
                return new ManagerResult<TTokenResponse>(ManagerErrors.Unauthorized);

            List<string> roles = (await userManager.GetRolesAsync(user)).ToList();

            return new ManagerResult<TTokenResponse>(
                GenerateTokenResponse<TTokenResponse, TKey>(appConfig, loginModel.Username, user.Id, roles,
                    additionalInfo));
        }
    }
}
