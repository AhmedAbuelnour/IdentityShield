using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.Auth.Register.Commands
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

        public class Handler(ShieldUserManager _userManager, RoleManager<IdentityRole> _roleManager, JwtTokenProvider<ShieldUser> _jwtTokenProvider) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _userManager.FindByProviderAsync(request.Request.Name, request.Request.Value) is not null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0013",
                        Description = "This account has been used before, please use another account."
                    }));
                }

                if (await _userManager.LinkProviderAsync(request.Request.Name, request.Request.Value) is ShieldUser shieldUser)
                {
                    if (!await _roleManager.RoleExistsAsync(request.Request.RoleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole
                        {
                            Name = request.Request.RoleName,
                            NormalizedName = request.Request.RoleName.ToUpperInvariant(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        });
                    }

                    await _userManager.AddToRoleAsync(shieldUser, request.Request.RoleName);

                    JwtResponse jwtResponse = await _jwtTokenProvider.GetJwtAsync(shieldUser, [request.Request.RoleName], cancellationToken);

                    return TypedResults.Ok(new OperationResult<Response>
                    {
                        Result = new Response
                        {
                            AccessToken = jwtResponse.AccessToken,
                            RefreshToken = jwtResponse.RefreshToken,
                            Email = shieldUser.Email,
                            PhoneNumber = shieldUser.PhoneNumber,
                            UserId = shieldUser.Id,
                            IsEmailVerified = true,
                            IsPhoneNumberVerified = true,
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
