using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Contracts;
public interface INotificationService
{
    Task<bool> SendEmailAsync<TUser>(TUser user, string mailSubject, string token, CancellationToken cancellationToken) where TUser : IdentityUser;
    Task<bool> SendEmailAsync(string userName, string email, string mailSubject, string token, CancellationToken cancellationToken);
    Task<bool> SendSMSAsync<TUser>(TUser user, string token, CancellationToken cancellationToken) where TUser : IdentityUser;
    Task<bool> SendSMSAsync(string phoneNumber, string token, CancellationToken cancellationToken);
}

