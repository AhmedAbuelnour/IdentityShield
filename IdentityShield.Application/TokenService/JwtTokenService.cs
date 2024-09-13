using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace IdentityShield.Application.TokenService
{
    public class JwtTokenService(IOptions<ShieldOptions> shieldOptions)
    {
        public string GenerateJwtToken<TUser>(TUser user, params string[] roles) where TUser : IdentityUser
        {
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ];

            foreach (KeyValuePair<string, string> claim in shieldOptions.Value.IdentityClaims)
            {
                if (ExtractPropertyValue(user, claim.Value) is string extractedValue && !string.IsNullOrEmpty(extractedValue))
                {
                    claims.Add(new Claim(claim.Key, extractedValue));
                }
            }

            foreach (string audience in shieldOptions.Value.Audiences)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
            }

            foreach (string role in roles ?? [])
            {
                claims.Add(new Claim(shieldOptions.Value.RoleClaimType, role));
            }


            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(issuer: shieldOptions.Value.Issuer,
                                                                                 claims: claims,
                                                                                 expires: DateTime.UtcNow.Add(shieldOptions.Value.AccessTokenExpiration),
                                                                                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(shieldOptions.Value.SecretKey)), SecurityAlgorithms.HmacSha256)));
        }

        private static string? ExtractPropertyValue<TUser>(TUser user, string propertyName)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));


            if (user.GetType().GetProperty(propertyName) is PropertyInfo propertyInfo && propertyInfo.GetValue(user, null) is object value)
            {
                return value.ToString();
            }
            else
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{user.GetType().Name}'");
            }
        }
    }

}
