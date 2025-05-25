using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace IdentityShield.Infrastructure.Implementations
{
    public sealed class DefaultIdentityClaimMapper : IIdentityClaimMapper<ShieldUser>
    {
        public IEnumerable<Claim> GetClaims(ShieldUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            ];

            // Add other custom claims here.
            if (!string.IsNullOrEmpty(user.UserName))
            {
                claims.Add(new Claim("username", user.UserName));
            }

            return claims;
        }
    }

}
