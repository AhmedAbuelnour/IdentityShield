using ErrorOr;
using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityShield.Application.UseCases.Realms.CreateRealm.Commands
{
    public class CreateRealmCommand : IEndPointRequest
    {
        [FromBody] public required CreateRealmRequest Request { get; set; }
    }

    public class CreateRealmCommandHandler(IRealmService _realmService) : IEndPointRequestHandler<CreateRealmCommand>
    {
        public async Task<IResult> Handle(CreateRealmCommand request, CancellationToken cancellationToken)
        {
            ErrorOr<bool> createResult = await _realmService.CreateAsync(request.Request, cancellationToken);

            if (createResult.IsError)
            {
                return Results.UnprocessableEntity(createResult.FirstError);
            }

            return Results.Ok();
        }
    }

}
