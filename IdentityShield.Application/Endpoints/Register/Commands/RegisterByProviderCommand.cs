using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Register.Commands
{
    public static class RegisterByProviderCommand
    {
        public class Request
        {
            public required string Name { get; set; }
            public required string Value { get; set; }
            public required string RoleName { get; set; }

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

        public class Handler(ShieldUserManager _userManager, RoleManager<IdentityRole> _roleManager, JwtTokenService _jwtTokenService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByProviderAsync(request.Request.Name, request.Request.Value);

                if (user is not null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0013",
                        Description = "This account has been used before, please use another account."
                    }));
                }

                user = new ApplicationUser
                {
                    UserName = $"{request.Request.Name}@{request.Request.Value}",
                };

                if (await _userManager.CreateAsync(user) is IdentityResult identityResult && !identityResult.Succeeded)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0001",
                        Description = "Can't find the user with the provided data"
                    }));
                }

                if (await _userManager.LinkProviderAsync(user, request.Request.Name, request.Request.Value) is IdentityResult linkProvider && linkProvider.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(request.Request.RoleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(request.Request.RoleName));
                    }

                    await _userManager.AddToRoleAsync(user, request.Request.RoleName);

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


                return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                {
                    Code = "XIDN0001",
                    Description = "Can't find the user with the provided data"
                }));
            }
        }
    }
}
