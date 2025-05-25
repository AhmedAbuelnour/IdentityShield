using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Auth.Register.Commands
{
    public static class RegisterByEmailCommand
    {
        public class Request
        {
            public required string Email { get; set; }
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
                             JwtTokenProvider<ShieldUser> _jwtTokenProvider) : IEndPointRequestHandler<Command>
        {

            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser? user = await _userManager.FindByEmailAsync(request.Request.Email);

                if (user is not null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0010",
                        Description = "This email is already existing, please use another one."
                    }));

                }

                user = new ShieldUser
                {
                    Email = request.Request.Email,
                    UserName = request.Request.Email
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
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = request.Request.RoleName,
                        NormalizedName = request.Request.RoleName.ToUpperInvariant(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                await _userManager.AddToRoleAsync(user, request.Request.RoleName);


                // generate otp

                await _userManager.SendEmailNotificationAsync(user, Constant.Purposes.Register, "Register -تسجيل حساب جديد", TimeSpan.FromHours(24));


                JwtResponse jwtResponse = await _jwtTokenProvider.GetJwtAsync(user, [request.Request.RoleName], cancellationToken);

                return TypedResults.Ok(new OperationResult<Response>
                {
                    Result = new Response
                    {
                        AccessToken = jwtResponse.AccessToken,
                        RefreshToken = jwtResponse.RefreshToken,
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
