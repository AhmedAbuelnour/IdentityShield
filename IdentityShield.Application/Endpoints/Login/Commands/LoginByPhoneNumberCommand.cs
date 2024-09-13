using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityShield.Application.Endpoints.Login.Commands
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
                             SignInManager<ApplicationUser> _signInManager,
                             IOptions<ShieldOptions> _shieldOptions,
                             JwtTokenService _jwtTokenService) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ApplicationUser? user = await _userManager.FindByPhoneNumberAsync(request.Request.PhoneNumber);

                if (user == null)
                {
                    return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                    {
                        Code = "XIDN0028",
                        Description = "User not found!"
                    }));
                }


                SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Request.Password, isPersistent: false, lockoutOnFailure: _shieldOptions.Value.LockoutOnFailure);

                if (result.Succeeded)
                {
                    IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);

                    return TypedResults.Ok(OperationResult<Response>.Succeed(new Response
                    {
                        AccessToken = _jwtTokenService.GenerateJwtToken(user, userRoles.ToArray()),
                        RefreshToken = await _userManager.GenerateRefreshTokenAsync(user),
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        UserId = user.Id,
                        IsEmailVerified = user.EmailConfirmed,
                        IsPhoneNumberVerified = user.PhoneNumberConfirmed,
                    }));
                }


                return TypedResults.UnprocessableEntity(OperationResult.Failed(new OperationError
                {
                    Code = "XIDN0001",
                    Description = "Invalid data. Please try again"
                }));
            }

        }
    }
}
