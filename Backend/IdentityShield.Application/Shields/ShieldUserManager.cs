using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace IdentityShield.Application.Shields
{
    public class ShieldUserManager(IUserStore<ShieldUser> store,
                                   IOptions<IdentityOptions> optionsAccessor,
                                   IPasswordHasher<ShieldUser> passwordHasher,
                                   IEnumerable<IUserValidator<ShieldUser>> userValidators,
                                   IEnumerable<IPasswordValidator<ShieldUser>> passwordValidators,
                                   ILookupNormalizer keyNormalizer,
                                   IdentityErrorDescriber errors,
                                   IServiceProvider services,
                                   IExternalProviderRepository _userProviderRepo,
                                   IShieldNotificationRepository _tokenRepository,
                                   IAccountLinkingRepository _accountLinkingRepo,
                                   INotificationService _notificationService,
                                   IOptions<ShieldOptions> _shieldOptions,
                                   IRefreshTokenRepository _refreshTokenRepo,
                                   ILogger<UserManager<ShieldUser>> logger) : UserManager<ShieldUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {


        public async Task<bool> ActiveSessionDetectedAsync<TUser>(TUser user) where TUser : ShieldUser
        {
            if (_shieldOptions.Value.SingleActiveSessionEnabled && await CheckActiveSessionAsync(user))
            {
                if (user.Attributes?.TryGetValue("EnforcedLogoutEnabled", out object? value) == true
                    && value is JsonElement enabledLogoutEnabledJson
                    && enabledLogoutEnabledJson.Deserialize<bool>())
                {
                    await LogoutAsync(user);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public Task<IEnumerable<ShieldUser>> GetManagedAccountsAsync<TUser>(TUser managerAccount) where TUser : ShieldUser
        {
            return _accountLinkingRepo.GetManagedAccountsAsync(managerAccount.Id, CancellationToken);
        }

        public Task<IEnumerable<ShieldUser>> GetManagedByAccountsAsync<TUser>(TUser managedAccount) where TUser : ShieldUser
        {
            return _accountLinkingRepo.GetManagedByAccountsAsync(managedAccount.Id, CancellationToken);
        }

        public Task<bool> IsAlreadyLinkedAsync<TUser>(TUser managerAccount, TUser managedAccount) where TUser : ShieldUser
        {
            return _accountLinkingRepo.IsActiveLinkedAsync(managerAccount.Id, managedAccount.Id, CancellationToken);
        }

        public async Task<int> LinkManagedAccountAsync<TUser>(TUser managerAccount, TUser managedAccount, string linkType = "Full-Control") where TUser : ShieldUser
        {
            if (await _accountLinkingRepo.IsDisabledLinkedAsync(managerAccount.Id, managedAccount.Id, CancellationToken))
            {
                return await _accountLinkingRepo.ReactivateLinkAsync(managerAccount.Id, managedAccount.Id, CancellationToken);
            }
            else
            {
                return await _accountLinkingRepo.LinkAsync(managerAccount.Id, managedAccount.Id, linkType, CancellationToken);
            }
        }

        public async Task<int> UnlinkManagedAccountAsync<TUser>(TUser managerAccount, TUser managedAccount) where TUser : ShieldUser
        {
            return await _accountLinkingRepo.UnlinkAsync(managerAccount.Id, managedAccount.Id, CancellationToken);
        }

        public async Task SendSMSNotificationAsync<TUser>(TUser user, string purpose, TimeSpan duration) where TUser : ShieldUser
        {
            if (await GetNotificationSentCountAsync(user, purpose, Constant.NotificationProvider.PhoneNumber, duration) > 5)
            {
                throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0012", "You have exceeded the maximum number of sms messages per day, please try again tomorrow.");
            }

            if (await AddNotificationAsync(user, Constant.NotificationProvider.PhoneNumber, purpose, user.PhoneNumber) > 0)
            {
                string token = await GenerateUserTokenAsync(user, Constant.TokenProviders.OtpTokenProvider, purpose);

                // send otp
                await _notificationService.SendSMSAsync(user, token, CancellationToken);
            }
        }

        public async Task SendEmailNotificationAsync<TUser>(TUser user, string purpose, string mailSubject, TimeSpan duration) where TUser : ShieldUser
        {
            if (await GetNotificationSentCountAsync(user, purpose, Constant.NotificationProvider.Email, duration) > 5)
            {
                throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0012", "You have exceeded the maximum number of emails messages per day, please try again tomorrow.");
            }

            if (await AddNotificationAsync(user, Constant.NotificationProvider.Email, purpose, user.Email) > 0)
            {
                string token = await GenerateUserTokenAsync(user, Constant.TokenProviders.OtpTokenProvider, purpose);

                // send otp
                await _notificationService.SendEmailAsync(user, mailSubject, token, CancellationToken);
            }
        }



        public Task<ShieldUser?> FindByPhoneNumberAsync(string emailOrPhoneNumber)
        {
            return Users.SingleOrDefaultAsync(u => u.PhoneNumber == emailOrPhoneNumber, CancellationToken);
        }


        public async Task<ShieldUser?> FindByProviderAsync(string name, string value)
        {
            if (await _userProviderRepo.GetUserIdAsync(name, value, CancellationToken).ConfigureAwait(false) is string userId)
            {
                return await FindByIdAsync(userId);
            }
            return null;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ShieldUser user, string newPassword)
        {
            string generatePasswordResetToken = await GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

            return await base.ResetPasswordAsync(user, generatePasswordResetToken, newPassword).ConfigureAwait(false);
        }

        public async Task<IdentityResult> ChangeEmailAsync(ShieldUser user, string newEmail)
        {
            string generatedToken = await GenerateChangeEmailTokenAsync(user, newEmail).ConfigureAwait(false);

            return await base.ChangeEmailAsync(user, newEmail, generatedToken).ConfigureAwait(false);
        }

        public async Task<IdentityResult> ChangePhoneNumberAsync(ShieldUser user, string newPhoneNumber)
        {
            string generatedToken = await GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber).ConfigureAwait(false);

            return await base.ChangePhoneNumberAsync(user, newPhoneNumber, generatedToken).ConfigureAwait(false);
        }


        public async Task<ShieldUser> LinkProviderAsync(string name, string value)
        {
            ShieldUser user = new ShieldUser
            {
                UserName = $"{name}@{value}",
            };

            if (await CreateAsync(user) is IdentityResult identityResult && !identityResult.Succeeded)
            {
                throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0001", "Can't find the user with the provided data");
            }

            if (await _userProviderRepo.CheckUniquenessAsync(name, value, CancellationToken).ConfigureAwait(false))
            {
                if (await _userProviderRepo.LinkProviderAsync(user, name, value, CancellationToken).ConfigureAwait(false) > 0)
                {
                    return user;
                }
            }

            throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0001", "Can't find the user with the provided data");
        }

        public async Task<ShieldUser> LinkProviderAsync(ShieldUser user, string name, string value)
        {
            if (await _userProviderRepo.CheckUniquenessAsync(name, value, CancellationToken).ConfigureAwait(false))
            {
                if (await _userProviderRepo.LinkProviderAsync(user, name, value, CancellationToken).ConfigureAwait(false) > 0)
                {
                    return user;
                }
            }

            throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0001", "Can't find the user with the provided data");
        }

        public async Task<IdentityResult> UnlinkProviderAsync(ShieldUser user, string name)
        {
            if (await _userProviderRepo.UnlinkProviderAsync(user, name, CancellationToken).ConfigureAwait(false) > 0)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ShieldUser user)
        {
            string generatedToken = await GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

            return await base.ConfirmEmailAsync(user, generatedToken).ConfigureAwait(false);

        }

        public Task<IdentityResult> ConfirmPhoneNumberAsync(ShieldUser user)
        {
            return ChangePhoneNumberAsync(user, user.PhoneNumber);
        }


        public async Task<ShieldUser?> ConsumeRefreshTokenAsync(string token)
        {
            if (await _refreshTokenRepo.ValidateRefreshTokenAsync(token, CancellationToken).ConfigureAwait(false))
            {
                if (await _refreshTokenRepo.GetUserByRefreshTokenAsync(token, CancellationToken).ConfigureAwait(false) is ShieldUser identityUser
                    && await _refreshTokenRepo.DeleteConsumedTokenAsync(token, CancellationToken).ConfigureAwait(false) > 0)
                {
                    return identityUser;
                }
            }

            return null;
        }

        public async Task<IdentityResult> LogoutAsync<TUser>(TUser user) where TUser : IdentityUser
        {
            if (await _refreshTokenRepo.LogoutAsync(user.Id, CancellationToken) > 0)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> LogoutAsync(string refreshToken)
        {
            if (await _refreshTokenRepo.GetUserByRefreshTokenAsync(refreshToken, CancellationToken) is ShieldUser user
                && await _refreshTokenRepo.LogoutAsync(user.Id, CancellationToken) > 0)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        private Task<bool> CheckActiveSessionAsync<TUser>(TUser user) where TUser : IdentityUser
        {
            return _refreshTokenRepo.CheckActiveSessionAsync(user, CancellationToken);
        }


        private Task<int> GetNotificationSentCountAsync<TUser>(TUser user, string purpose, string provider, TimeSpan duration) where TUser : IdentityUser
        {
            return _tokenRepository.GetSentCountAsync(user, purpose, provider, duration, CancellationToken);
        }

        private Task<int> AddNotificationAsync<TUser>(TUser user, string provider, string purpose, string recipient) where TUser : IdentityUser
        {
            return _tokenRepository.AddAsync(new ShieldNotification
            {
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Provider = provider,
                Purpose = purpose,
                Recipient = recipient,
            }, CancellationToken);
        }

    }
}
