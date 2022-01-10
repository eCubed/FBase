using FBase.Api;
using FBase.Api.EntityFramework;
using FBase.Api.Server.Models;
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

namespace FBase.Api.Server.Utils;

public static class AuthenticationTools
{
    public static async Task<TokenResponse> GenerateTokenResponseAsync<TApiServerConfig, TUser, TUserKey>(
        TApiServerConfig config,
        UserManager<TUser> userManager,
        TUser user,
        RefreshTokenManager<RefreshToken<TUser, TUserKey>, TUserKey> refreshTokenManager,
        RefreshToken<TUser, TUserKey>? refreshToken = null,
        Func<TUser, List<Claim>>? addAdditionalClaims = null)
        where TApiServerConfig : class, IApiServerConfig, new()
        where TUserKey : IEquatable<TUserKey>
        where TUser : IdentityUser<TUserKey>, new()
    {
        List<string> roles = (await userManager.GetRolesAsync(user)).ToList();
        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? ""));
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
            return new TokenResponse
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

        return new TokenResponse
        {
            Username = user.UserName,
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            RefreshToken = createRes.Data.Token,
            Roles = roles,
            ExpiresAt = securityToken.ValidTo
        };
    }

    public static async Task<TokenResponse> VerifyAndGenerateTokenAsync<TApiServerConfig, TUser, TUserKey>(
        RefreshTokenRequest refreshTokenRequest,
        TApiServerConfig config,
        RefreshTokenManager<RefreshToken<TUser, TUserKey>, TUserKey> refreshTokenManager,
        UserManager<TUser> userManager,
        TokenValidationParameters tokenValidationParameters,
        Func<TUser, List<Claim>>? addAdditionalClaims = null)
        where TApiServerConfig : class, IApiServerConfig, new()
        where TUserKey : IEquatable<TUserKey>
        where TUser : IdentityUser<TUserKey>, new()
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var refreshToken = await refreshTokenManager.FindByTokenAsync(refreshTokenRequest.RefreshToken);
        var user = await userManager.FindByIdAsync(refreshToken.UserId.ToString());

        Func<TUser, List<Claim>> addAdditionalClaimsIncludingAppIdIfAny = (user) =>
        {
            List<Claim> claims = new List<Claim>();
            if (addAdditionalClaims != null)
                claims.AddRange(addAdditionalClaims.Invoke(user));

            JwtSecurityToken jwtSecurityToken = jwtTokenHandler.ReadJwtToken(refreshTokenRequest.Token);
            return claims;
        };

        try
        {
            var tokenCheckResult = jwtTokenHandler.ValidateToken(
                token: refreshTokenRequest.Token,
                validationParameters: tokenValidationParameters,
                out var validatedToken);

            return await GenerateTokenResponseAsync(
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
                return await GenerateTokenResponseAsync(
                    config: config,
                    userManager: userManager,
                    refreshTokenManager: refreshTokenManager,
                    user: user,
                    refreshToken: refreshToken,
                    addAdditionalClaims: addAdditionalClaimsIncludingAppIdIfAny);
            }
            else
            {
                return await GenerateTokenResponseAsync(
                    config: config,
                    userManager: userManager,
                    refreshTokenManager: refreshTokenManager,
                    user: user,
                    refreshToken: null,
                    addAdditionalClaims: addAdditionalClaimsIncludingAppIdIfAny);
            }
        }
    }

    public static async Task<ManagerResult<TokenResponse>> LoginAsync<TApiServerConfig, TUser, TUserKey>(
        LoginModel loginModel,
        TApiServerConfig config,
        UserManager<TUser> userManager,
        RefreshTokenManager<RefreshToken<TUser, TUserKey>, TUserKey> refreshTokenManager)
        where TApiServerConfig : class, IApiServerConfig, new()
        where TUserKey : IEquatable<TUserKey>
        where TUser : IdentityUser<TUserKey>, new()
    {
        var user = await userManager.FindByNameAsync(loginModel.Username);

        if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
        {
            var tokenResponse = await GenerateTokenResponseAsync(
                 config: config,
                 userManager: userManager,
                 user: user,
                 refreshToken: null,
                 refreshTokenManager: refreshTokenManager
             );

            return new ManagerResult<TokenResponse>(tokenResponse);
        }
        else
        {
            return new ManagerResult<TokenResponse>(ManagerErrors.Unauthorized);
        }
    }
}
