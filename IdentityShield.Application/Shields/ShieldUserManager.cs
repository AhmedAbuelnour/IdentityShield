using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityShield.Application.Shields
{
    public class ShieldUserManager(IUserStore<ApplicationUser> store,
                                   IOptions<IdentityOptions> optionsAccessor,
                                   IPasswordHasher<ApplicationUser> passwordHasher,
                                   IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                                   IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                                   ILookupNormalizer keyNormalizer,
                                   IdentityErrorDescriber errors,
                                   IServiceProvider services,
                                   IIdentityUserProviderRepository _userProviderRepo,
                                   IRefreshTokenRepository _refreshTokenRepo,
                                   ILogger<UserManager<ApplicationUser>> logger) : UserManager<ApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {

        public Task<int> GetTokenCountAsync<TUser>(TUser user, string purpose, string provider, TimeSpan duration, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            return _userProviderRepo.GetTokenCountAsync(user, purpose, provider, duration, cancellationToken);
        }

        public Task<int> AddTokenAsync<TUser>(TUser user, string purpose, string token, string provider, CancellationToken cancellationToken) where TUser : IdentityUser
        {
            return _userProviderRepo.AddTokenAsync(user, purpose, token, provider, cancellationToken);
        }

        public Task<ApplicationUser?> FindByPhoneNumberAsync(string emailOrPhoneNumber)
        {
            return Users.SingleOrDefaultAsync(u => u.PhoneNumber == emailOrPhoneNumber, CancellationToken);
        }


        public async Task<ApplicationUser?> FindByProviderAsync(string name, string value)
        {
            if (await _userProviderRepo.GetUserIdAsync(name, value, CancellationToken).ConfigureAwait(false) is string userId)
            {
                return await FindByIdAsync(userId);
            }
            return null;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            string generatePasswordResetToken = await GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

            return await base.ResetPasswordAsync(user, generatePasswordResetToken, newPassword).ConfigureAwait(false);
        }

        public async Task<IdentityResult> ChangeEmailAsync(ApplicationUser user, string newEmail)
        {
            string generatedToken = await GenerateChangeEmailTokenAsync(user, newEmail).ConfigureAwait(false);

            return await base.ChangeEmailAsync(user, newEmail, generatedToken).ConfigureAwait(false);
        }

        public async Task<IdentityResult> ChangePhoneNumberAsync(ApplicationUser user, string newPhoneNumber)
        {
            string generatedToken = await GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber).ConfigureAwait(false);

            return await base.ChangePhoneNumberAsync(user, newPhoneNumber, generatedToken).ConfigureAwait(false);
        }


        public async Task<IdentityResult> LinkProviderAsync(ApplicationUser user, string name, string value)
        {
            if (await _userProviderRepo.CheckUniquenessAsync(name, value, CancellationToken).ConfigureAwait(false))
            {
                if (await _userProviderRepo.LinkProviderAsync(user, name, value, CancellationToken).ConfigureAwait(false) > 0)
                {
                    return IdentityResult.Success;
                }
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> UnlinkProviderAsync(ApplicationUser user, string name)
        {
            if (await _userProviderRepo.UnlinkProviderAsync(user, name, CancellationToken).ConfigureAwait(false) > 0)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user)
        {
            string generatedToken = await GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

            return await base.ConfirmEmailAsync(user, generatedToken).ConfigureAwait(false);

        }

        public Task<IdentityResult> ConfirmPhoneNumberAsync(ApplicationUser user)
        {
            return ChangePhoneNumberAsync(user, user.PhoneNumber);
        }

        public Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
        {
            return _refreshTokenRepo.GenerateRefreshTokenAsync(user, CancellationToken);
        }

        public async Task<ApplicationUser?> ConsumeRefreshTokenAsync(string token)
        {
            if (await _refreshTokenRepo.ValidateRefreshTokenAsync(token, CancellationToken).ConfigureAwait(false))
            {
                if (await _refreshTokenRepo.GetUserByRefreshTokenAsync(token, CancellationToken).ConfigureAwait(false) is ApplicationUser identityUser
                    && await _refreshTokenRepo.DeleteConsumedTokenAsync(token, CancellationToken).ConfigureAwait(false) > 0)
                {
                    return identityUser;
                }
            }

            return null;
        }
    }
}
