using FBase.ApiServer.OAuth;
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
        public static async Task<TTokenResponse> GenerateTokenResponse<TUser, TUserKey, TRefreshToken, TTokenResponse>(
            IApiServerConfig config,
            RefreshTokenManager<TRefreshToken, TUserKey> refreshTokenManager,
            TUser user,
            List<string> roles,
            TRefreshToken refreshToken = null,
            Func<TUser, List<Claim>> addAdditionalClaims = null)
            where TUser : IdentityUser<TUserKey>
            where TUserKey : IEquatable<TUserKey>
            where TRefreshToken : class, IRefreshToken<TUserKey>, new()
            where TTokenResponse : class, ITokenResponse, new()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            roles.ForEach(roleName =>
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            });

            if (addAdditionalClaims != null)
            {
                claims.AddRange(addAdditionalClaims.Invoke(user));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.CryptionKey));

            var securityToken = new JwtSecurityToken(
                issuer: config.Issuer,
                audience: config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            if (refreshToken != null)
            {
                return new TTokenResponse
                {
                    Username = user.UserName,
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token,
                    Roles = roles,
                    ExpiresAt = securityToken.ValidTo
                };
            }

            var createRes = await refreshTokenManager.CreateAsync(
                jwtId: securityToken.Id,
                userId: user.Id);

            return new TTokenResponse
            {
                Username = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                RefreshToken = createRes.Data.Token,
                Roles = roles,
                ExpiresAt = securityToken.ValidTo
            };
        }

        public static async Task<TTokenResponse> GenerateTokenResponseAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
            IApiServerConfig config,
            UserManager<TUser> userManager,
            RefreshTokenManager<TRefreshToken, TUserKey> refreshTokenManager,
            TUser user,
            TRefreshToken refreshToken = null,
            Func<TUser, List<Claim>> addAdditionalClaims = null)
            where TUser : IdentityUser<TUserKey>
            where TUserKey : IEquatable<TUserKey>
            where TRefreshToken : class, IRefreshToken<TUserKey>, new()
            where TTokenResponse: class, ITokenResponse, new()
        {
            List<string> roles = (await userManager.GetRolesAsync(user)).ToList();
            return await GenerateTokenResponse<TUser, TUserKey, TRefreshToken, TTokenResponse>(config, refreshTokenManager, user, roles, refreshToken, addAdditionalClaims);
        }

        public static async Task<TTokenResponse> VerifyAndGenerateTokenAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
            RefreshTokenRequest refreshTokenRequest,
            IApiServerConfig config,
            RefreshTokenManager<TRefreshToken, TUserKey> refreshTokenManager,
            UserManager<TUser> userManager,
            TokenValidationParameters tokenValidationParameters,
            Func<TUser, List<Claim>> addAdditionalClaims = null)
            where TUser : IdentityUser<TUserKey>
            where TRefreshToken : class, IRefreshToken<TUserKey>, new()
            where TTokenResponse: class, ITokenResponse, new()
            where TUserKey : IEquatable<TUserKey>
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var refreshToken = await refreshTokenManager.FindByTokenAsync(refreshTokenRequest.RefreshToken);
            var user = await userManager.FindByIdAsync(refreshToken.UserId.ToString());

            Func<TUser, List<Claim>> addAdditionalClaimsIncludingAppIdIfAny = (user) => {
                List<Claim> claims = new List<Claim>();
                if (addAdditionalClaims != null)
                    claims.AddRange(addAdditionalClaims.Invoke(user));

                JwtSecurityToken jwtSecurityToken = jwtTokenHandler.ReadJwtToken(refreshTokenRequest.Token);

                Claim appIdClaim = jwtSecurityToken.Claims.SingleOrDefault(c => c.Type == OAuthClaimTypes.ApplicationId);
                if (appIdClaim != null)
                {
                    claims.Add(appIdClaim);
                }
                return claims;
            };

            try
            {
                var tokenCheckResult = jwtTokenHandler.ValidateToken(
                    token: refreshTokenRequest.Token,
                    validationParameters: tokenValidationParameters,
                    out var validatedToken);

                return await GenerateTokenResponseAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
                        config: config,
                        userManager: userManager,
                        refreshTokenManager: refreshTokenManager,
                        user: user,
                        refreshToken: refreshToken,
                        addAdditionalClaims: addAdditionalClaimsIncludingAppIdIfAny);
            }
            catch (SecurityTokenException)
            {
                if (refreshToken.ExpirationDate >= DateTime.UtcNow)
                {
                    return await GenerateTokenResponseAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
                        config: config,
                        userManager: userManager,
                        refreshTokenManager: refreshTokenManager,
                        user: user,
                        refreshToken: refreshToken,
                        addAdditionalClaims: addAdditionalClaimsIncludingAppIdIfAny);
                }
                else
                {
                    return await GenerateTokenResponseAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
                        config: config,
                        userManager: userManager,
                        refreshTokenManager: refreshTokenManager,
                        user: user,
                        refreshToken: null,
                        addAdditionalClaims: addAdditionalClaimsIncludingAppIdIfAny);
                }
            }
        }

        public static async Task<ManagerResult<TTokenResponse>> LoginAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
            LoginModel loginModel,
            IApiServerConfig config,
            UserManager<TUser> userManager,
            RefreshTokenManager<TRefreshToken, TUserKey> refreshTokenManager)
            where TUser: IdentityUser<TUserKey>
            where TUserKey: IEquatable<TUserKey>
            where TRefreshToken : class, IRefreshToken<TUserKey>, new()
            where TTokenResponse : class, ITokenResponse, new()
        {

            var user = await userManager.FindByNameAsync(loginModel.Username);

            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
               var tokenResponse = await GenerateTokenResponseAsync<TUser, TUserKey, TRefreshToken, TTokenResponse>(
                    config: config,
                    userManager: userManager,
                    user: user,
                    refreshToken: null,
                    refreshTokenManager: refreshTokenManager
                );

                return new ManagerResult<TTokenResponse>(tokenResponse);
            }
            else
            {
                return new ManagerResult<TTokenResponse>(ApiServerMessages.Unauthorized);
            }
        }
    }
}
