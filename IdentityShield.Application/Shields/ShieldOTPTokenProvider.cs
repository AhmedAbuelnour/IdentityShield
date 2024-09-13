using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityShield.Application.Shields
{
    public class ShieldOTPTokenProvider<TUser>(IDataProtectionProvider dataProtectionProvider,
                                               IOptions<CustomOtpTokenProviderOptions> options,
                                               IOptions<ShieldOptions> shieldOptions,
                                               ILogger<ShieldOTPTokenProvider<TUser>> logger) : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger) where TUser : IdentityUser
    {
        private const int Digits = 4;
        private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector(Constant.TokenProviders.OtpToken);
        private readonly ShieldOptions _shieldOptions = shieldOptions.Value;

        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            string otp = Random.Shared.Next(MultiplyNTimes(Digits), MultiplyNTimes(Digits + 1) - 1).ToString("D" + Digits);

            string token = $"{user.Id}:{otp}:{DateTime.UtcNow.Ticks}";

            // Save the token in AspNetUserTokens
            await manager.SetAuthenticationTokenAsync(user, Constant.TokenProviders.OtpToken, purpose, _protector.Protect(token));

            return otp;
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            try
            {
                string? protectedToken = await manager.GetAuthenticationTokenAsync(user, Constant.TokenProviders.OtpToken, purpose);

                if (string.IsNullOrEmpty(protectedToken))
                {
                    return false;
                }

                string unprotectedToken = _protector.Unprotect(protectedToken); // Unprotect (decrypt) the token

                string[] tokenParts = unprotectedToken.Split(':');

                string userId = tokenParts[0]; // get the user part

                if (!user.Id.Equals(userId))
                {
                    return false;
                }

                string otp = tokenParts[1]; // get the otp part

                if (!otp.Equals(token))
                {
                    return false;
                }


                DateTime tokenTimestamp = new(long.Parse(tokenParts[2])); // get the time part

                if ((DateTime.UtcNow - tokenTimestamp).TotalSeconds > _shieldOptions.OTPTokenExpiry.TotalSeconds)
                {
                    return false;
                }

                await manager.RemoveAuthenticationTokenAsync(user, Constant.TokenProviders.OtpToken, purpose).ConfigureAwait(false);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int MultiplyNTimes(int n)
        {
            if (n == 1)
                return 1;
            else
                return 10 * MultiplyNTimes(n - 1);
        }
    }

    public class CustomOtpTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        // Custom options if needed
    }
}
