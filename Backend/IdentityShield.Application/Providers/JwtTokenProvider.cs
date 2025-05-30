using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Text;

namespace IdentityShield.Application.Providers
{
    public sealed class JwtTokenProvider<TUser>(IOptions<ShieldOptions> shieldOptions,
                                                IRefreshTokenRepository _refreshTokenRepo,
                                                IIdentityClaimMapper<TUser> claimMapper,
                                                IOpenIddictTokenGenerator _tokenGenerator) where TUser : IdentityUser
    {
        public async Task<JwtResponse> GetJwtAsync(TUser user, string[] roles, CancellationToken cancellationToken = default)
        {
            ShieldRefreshToken identityRefreshToken = await _refreshTokenRepo.GenerateRefreshTokenAsync(user, cancellationToken);

            return new JwtResponse
            {
                RefreshToken = identityRefreshToken.Token,
                AccessToken = await GetAccessTokenAsync(shieldOptions, claimMapper, user, roles, identityRefreshToken.SessionId, cancellationToken)
            };
        }

        private async Task<string> GetAccessTokenAsync(IOptions<ShieldOptions> shieldOptions, IIdentityClaimMapper<TUser> claimMapper, TUser user, string[] roles, Guid? sessionId, CancellationToken cancellationToken)
        {
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ];

            claims.AddRange(claimMapper.GetClaims(user));

            foreach (string audience in shieldOptions.Value.Audiences)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
            }

            foreach (string role in roles ?? [])
            {
                claims.Add(new Claim(shieldOptions.Value.RoleClaimType, role));
            }

            if (sessionId.HasValue)
            {
                claims.Add(new Claim(shieldOptions.Value.SessionClaimType, sessionId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, shieldOptions.Value.NameClaimType, shieldOptions.Value.RoleClaimType);
            var principal = new ClaimsPrincipal(identity);

            principal.SetScopes([OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.OfflineAccess]);

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
            }

            return await _tokenGenerator.CreateAccessTokenAsync(principal, cancellationToken: cancellationToken);
        }
    }

}
