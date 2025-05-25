using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace IdentityShield.Application.Providers
{
    public sealed class JwtTokenProvider<TUser>(IOptions<ShieldOptions> shieldOptions,
                                                IRefreshTokenRepository _refreshTokenRepo,
                                                IIdentityClaimMapper<TUser> claimMapper) where TUser : IdentityUser
    {
        public async Task<JwtResponse> GetJwtAsync(TUser user, string[] roles, CancellationToken cancellationToken = default)
        {
            ShieldRefreshToken identityRefreshToken = await _refreshTokenRepo.GenerateRefreshTokenAsync(user, cancellationToken);

            return new JwtResponse
            {
                RefreshToken = identityRefreshToken.Token,
                AccessToken = GetAccessToken(shieldOptions, claimMapper, user, roles, identityRefreshToken.SessionId)
            };
        }

        private static string GetAccessToken(IOptions<ShieldOptions> shieldOptions, IIdentityClaimMapper<TUser> claimMapper, TUser user, string[] roles, Guid? sessionId)
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

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(shieldOptions.Value.AccessTokenExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(shieldOptions.Value.SecretKey)), SecurityAlgorithms.HmacSha256),
                Issuer = shieldOptions.Value.Issuer,
                IssuedAt = DateTime.UtcNow,
            };

            JsonWebTokenHandler jsonWebTokenHandler = new();

            return jsonWebTokenHandler.CreateToken(tokenDescriptor);
        }
    }

}
