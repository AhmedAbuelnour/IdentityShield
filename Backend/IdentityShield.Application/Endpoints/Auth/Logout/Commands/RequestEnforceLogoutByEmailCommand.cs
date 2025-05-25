using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityShield.Application.Endpoints.Auth.Logout.Commands
{
    public static class RequestEnforceLogoutByEmailCommand
    {
        public class Request
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class Response
        {
            public required string UserId { get; set; }
        }


        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public sealed class Handler(ShieldUserManager _userManager,
                             SignInManager<ShieldUser> _signInManager,
                             IOptions<ShieldOptions> _shieldOptions,
                             INotificationService _notificationService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser? user = await _userManager.FindByEmailAsync(request.Request.Email) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Request.Password, isPersistent: false, lockoutOnFailure: _shieldOptions.Value.LockoutOnFailure);

                if (result.Succeeded)
                {
                    await _userManager.SendEmailNotificationAsync(user, Constant.Purposes.LogoutRequest, "Logout Request - طلب تسجيل خروج", TimeSpan.FromHours(24));

                    return TypedResults.Ok(OperationResult<Response>.Succeed(new Response
                    {
                        UserId = user.Id
                    }));
                }
                else
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }
            }
        }

    }

}
