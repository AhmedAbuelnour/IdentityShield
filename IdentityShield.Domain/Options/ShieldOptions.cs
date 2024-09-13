using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Domain.Options
{
    public class ShieldOptions
    {
        /// <summary>
        /// Gets or sets the audience for token validation.
        /// </summary>
        public string[] Audiences { get; set; } = [];

        /// <summary>
        /// Gets or sets the issuer for token validation.
        /// </summary>
        public string Issuer { get; set; } = default!;

        /// <summary>
        /// Gets or sets the claim type for roles.
        /// </summary>
        public string RoleClaimType { get; set; } = "_roles";

        /// <summary>
        /// Gets or sets the claim type for the user's name.
        /// </summary>
        public string NameClaimType { get; set; } = "name";

        /// <summary>
        /// Gets or sets the access token expiration timespan, default is 15 minutes.
        /// </summary>
        public TimeSpan AccessTokenExpiration { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Gets or sets the refresh token expiration timespan, default is 30 minutes.
        /// </summary>
        public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Gets or sets the otp token expiration timespan, default is 30 minutes.
        /// </summary>
        public TimeSpan OTPTokenExpiry { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        public string SecretKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the ConnectionString.
        /// </summary>
        public string ConnectionString { get; set; } = default!;

        /// <summary>
        /// Gets or sets the identity claims to be extracted from the IdentityUser to be added to the token.
        /// </summary>
        public Dictionary<string, string> IdentityClaims { get; set; } = [];

        /// <summary>
        /// Options for configuring user lockout.
        /// </summary>
        public LockoutOptions? LockoutOptions { get; set; }


        /// <summary>
        /// Gets or sets the LockoutOnFailure that is preventing the user to exceed the number of available tries.
        /// </summary>
        public bool LockoutOnFailure { get; set; } = true;

        public string HashKey { get; set; } = default!;

        /// <summary>
        /// Validates the Shield options.
        /// </summary>
        public void Validate()
        {
            if (Audiences.Length == 0)
            {
                throw new ArgumentException("Audiences must be set in the Shield options.");
            }
            if (string.IsNullOrEmpty(Issuer))
            {
                throw new ArgumentException("Issuer must be set in the Shield options.");
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                throw new ArgumentException("SecretKey must be set in the Shield options.");
            }
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentException("ConnectionString must be set in the Shield options.");
            }
            if (string.IsNullOrEmpty(HashKey))
            {
                throw new ArgumentException("HashKey must be set in the Shield options.");
            }
        }
    }
}
