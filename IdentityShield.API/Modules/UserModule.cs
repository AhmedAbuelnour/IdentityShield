using Flaminco.MinimalMediatR.Abstractions;
using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application.Endpoints.User.Queries;

namespace IdentityShield.API.Modules
{
    public class UserModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/Shield/User").WithTags("User");

            group.MediateGet<GetUserInfoQuery.Query>("/GetUser/{UserId}").WithName("User-GetInfo");
        }
    }
}
