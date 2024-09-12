using ErrorOr;
using Flaminco.MinimalMediatR.Abstractions;
using IdentityShield.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityShield.Application.UseCases.Clients.CreateClient.Commands
{
    public class CreateClientCommand : IEndPointRequest
    {
        [FromBody] public required CreateClientRequest Request { get; set; }
    }

    public class CreateClientCommandHandler(IClientService _clientService) : IEndPointRequestHandler<CreateClientCommand>
    {
        public async Task<IResult> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            ErrorOr<bool> createResult = await _clientService.CreateAsync(request.Request, cancellationToken);

            if (createResult.IsError)
            {
                return Results.UnprocessableEntity(createResult.FirstError);
            }

            return Results.Ok();
        }
    }
}
