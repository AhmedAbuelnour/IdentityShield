using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Commands;
using IdentityShield.Application.Endpoints.AccessControl.LinkAccount.Queries;

namespace IdentityShield.API.Modules
{
    public class AccountLinkingModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/AccountLinking").AllowAnonymous().WithTags("Account-Linking-Management");

            group.MediatePost<CreateManagedAccountCommand.Command>("/Create").WithName("Account-Linking-Create");

            group.MediatePost<LinkManagedAccountCommand.Command>("/Link").WithName("Account-Linking-Link");

            group.MediateDelete<RemoveManagedAccountCommand.Command>("/Unlink").WithName("Account-Linking-Unlink");

            group.MediateGet<GetManagedAccountsQuery.Query>("/Managed/All").WithName("Account-Linking-Managed-Get-All");

            group.MediateGet<GetManagedByAccountsQuery.Query>("/ManagedBy/All").WithName("Account-Linking-ManagedBy-Get-All");
        }
    }
}
