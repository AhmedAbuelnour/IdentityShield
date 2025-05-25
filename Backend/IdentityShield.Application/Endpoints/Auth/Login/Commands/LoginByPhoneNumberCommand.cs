using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Models;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityShield.Application.Endpoints.Auth.Login.Commands
{
    public static class LoginByPhoneNumberCommand
    {
        public class Request
        {
            public required string PhoneNumber { get; set; }
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
                             SignInManager<ShieldUser> _signInManager,
                             IOptions<ShieldOptions> _shieldOptions,
                             JwtTokenProvider<ShieldUser> _jwtTokenProvider) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser? user = await _userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!"); ;

                SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Request.Password, isPersistent: false, lockoutOnFailure: _shieldOptions.Value.LockoutOnFailure);

                if (result.Succeeded)
                {
                    if (await _userManager.ActiveSessionDetectedAsync(user))
                    {
                        throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0030", "User already has an active session!");
                    }

                    IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);

                    JwtResponse jwtResponse = await _jwtTokenProvider.GetJwtAsync(user, userRoles.ToArray(), cancellationToken);

                    return TypedResults.Ok(OperationResult<Response>.Succeed(new Response
                    {
                        AccessToken = jwtResponse.AccessToken,
                        RefreshToken = jwtResponse.RefreshToken,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        UserId = user.Id,
                        IsEmailVerified = user.EmailConfirmed,
                        IsPhoneNumberVerified = user.PhoneNumberConfirmed,
                    }));
                }
                else
                {
                    throw new BusinessException(StatusCodes.Status422UnprocessableEntity, "XIDN0001", "Invalid data. Please try again!");
                }
            }

        }
    }
}
