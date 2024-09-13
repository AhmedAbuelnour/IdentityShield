using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Application.Contracts;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Register.Commands
{
    public static class RegisterByPhoneNumberCommand
    {
        public class Request
        {
            public required string PhoneNumber { get; set; }
            public required string RoleName { get; set; }
            public required string Password { get; set; }
        }

        public class Response
        {
            public string UserId { get; set; }
            public string? Email { get; set; }
            public bool? IsEmailVerified { get; set; }
            public string? PhoneNumber { get; set; }
            public bool? IsPhoneNumberVerified { get; set; }
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager,
                             RoleManager<IdentityRole> _roleManager,
                             INotificationService _notificationService,
                             JwtTokenService _jwtTokenService) : IEndPointRequestHandler<Command>
        {

            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber);

                if (user is not null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0011",
                        Description = "This phone number is already existing, please use another number."
                    }));
                }

                user = new ApplicationUser
                {
                    PhoneNumber = request.Request.PhoneNumber,
                    UserName = request.Request.PhoneNumber
                };

                if (await _userManager.CreateAsync(user, request.Request.Password) is IdentityResult identityResult && !identityResult.Succeeded)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0001",
                        Description = "Can't find the user with the provided data"
                    }));
                }

                if (!await _roleManager.RoleExistsAsync(request.Request.RoleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(request.Request.RoleName));
                }

                await _userManager.AddToRoleAsync(user, request.Request.RoleName);

                // generate otp

                string token = await _userManager.GenerateUserTokenAsync(user, Constant.TokenProviders.OtpToken, Constant.Purposes.Register);

                // send otp

                if (await _userManager.AddTokenAsync(user, Constant.Purposes.Register, token, Constant.TokenTrackerProvider.PhoneNumber, cancellationToken) > 0)
                {
                    // send otp
                    await _notificationService.SendSMSAsync(user, token, cancellationToken);
                }


                return TypedResults.Ok(new OperationResult<Response>
                {
                    Result = new Response
                    {
                        AccessToken = _jwtTokenService.GenerateJwtToken(user, request.Request.RoleName),
                        RefreshToken = await _userManager.GenerateRefreshTokenAsync(user),
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        UserId = user.Id,
                        IsEmailVerified = user.EmailConfirmed,
                        IsPhoneNumberVerified = user.PhoneNumberConfirmed,
                    }
                });


            }
        }


    }

}
