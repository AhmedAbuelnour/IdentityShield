using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityShield.Application.Contracts
{
    public interface IIdentityClaimMapper<in TUser> where TUser : IdentityUser
    {
        /// <summary>
        /// Gets the claims for the given user.
        /// </summary>
        /// <param name="user">The user to extract claims from.</param>
        /// <returns>A collection of claims to be added to the token.</returns>
        IEnumerable<Claim> GetClaims(TUser user);
    }
}
