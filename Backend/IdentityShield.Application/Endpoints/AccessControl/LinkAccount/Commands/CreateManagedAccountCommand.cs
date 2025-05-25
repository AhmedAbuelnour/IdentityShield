using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Exceptions;
using IdentityShield.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Commands
{
    public static class CreateManagedAccountCommand
    {
        public class Request
        {
            public required string ManagerId { get; set; }
            public required string UserName { get; set; }
            public required string RoleName { get; set; }
        }

        public class Command : IEndPointRequest
        {
            [FromBody] public required Request Request { get; set; }
        }

        public sealed class Handler(ShieldUserManager _userManager, RoleManager<IdentityRole> _roleManager) : IEndPointRequestHandler<Command>
        {
            public async Task<IResult> Handle(Command request, CancellationToken cancellationToken)
            {
                ShieldUser manager = await _userManager.FindByIdAsync(request.Request.ManagerId) ?? throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0028", "User not found!");

                ShieldUser managed = new()
                {
                    UserName = request.Request.UserName,
                };

                if (await _userManager.CreateAsync(managed) is IdentityResult identityResult && identityResult.Succeeded)
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

                    await _userManager.AddToRoleAsync(managed, request.Request.RoleName);

                    await _userManager.LinkManagedAccountAsync(manager, managed);

                    return TypedResults.Ok();
                }
                else
                {
                    throw new BusinessException(StatusCodes.Status400BadRequest, "XIDN0029", "Failed to create managed account!");
                }
            }
        }
    }
}
