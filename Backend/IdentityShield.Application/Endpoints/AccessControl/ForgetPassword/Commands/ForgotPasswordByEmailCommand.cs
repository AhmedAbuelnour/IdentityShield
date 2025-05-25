using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Endpoints.AccessControl.ForgetPassword.Commands
{
    public static class ForgotPasswordByEmailCommand
    {
        public class Request
        {
            public required string Email { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public class Handler(ShieldUserManager _userManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser user = await _userManager.FindByEmailAsync(request.Request.Email) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                await _userManager.SendEmailNotificationAsync(user, Constant.Purposes.ForgotPassword, "Forgot Password - فقدان كلمة المرور", TimeSpan.FromHours(24));


                return TypedResults.Ok(OperationResult.Success);
            }
        }
    }
}
