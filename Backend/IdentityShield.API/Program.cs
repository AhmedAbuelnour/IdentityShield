using IdentityShield.Application;
using IdentityShield.Application.UseCases.Clients.CreateClient.Commands;
using IdentityShield.Application.UseCases.Realms.CreateRealm.Commands;
using IdentityShield.Domain.Entities;
using IdentityShield.Infrastructure;
using IdentityShield.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationDIContainer().AddInfrastructureDIContainer();



var app = builder.Build();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
    IdentityModelEventSource.LogCompleteSecurityArtifact = true;



}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/db", async (ApplicationDbContext _dbContext, [FromServices] RoleManager<ApplicationRole> roleManager) =>
{
    await _dbContext.Database.EnsureCreatedAsync();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = "Admin",
            RealmId = Guid.Parse("64659158-FFB5-489A-BDEC-BC28B3D9B734")
        });
    }


})
.WithName("db")
.WithOpenApi();

app.MapGet("/relem", async (ISender _sender) =>
{
    await _sender.Send(new CreateRealmCommand
    {
        Request = new CreateRealmRequest
        {
            Name = "TEST",
            Description = "JUST FOR TESTING"
        }
    });

})
.WithName("realm")
.WithOpenApi();

app.MapGet("/client", async (ISender _sender) =>
{
    await _sender.Send(new CreateClientCommand
    {
        Request = new CreateClientRequest
        {
            ClientId = "test-client",
            ClientSecret = "c01655c2-fb96-4f4f-af60-b21ba051f01c",
            Permissions = [Guid.Parse("3B3BD9D9-3387-43CF-A102-F54F9454E74E")],
            DisplayName = "test",
            RealmId = Guid.Parse("64659158-FFB5-489A-BDEC-BC28B3D9B734")
        }
    });
})
.WithName("client")
.WithOpenApi();


app.MapPost("/connect/token", async (HttpContext httpContext) =>
{
    OpenIddictRequest? request = httpContext.GetOpenIddictServerRequest();

    if (request is null)
    {
        return Results.BadRequest("Invalid request.");
    }

    // Handle the Client Credentials grant type
    if (request.IsPasswordGrantType() || request.IsRefreshTokenGrantType())
    {
        ClaimsIdentity identity = new(
                     authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                     nameType: Claims.Name,
        roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, request.ClientId);
        identity.AddClaim(Claims.Name, "Display Name");
        identity.AddClaim(Claims.Role, "admin");

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(new[]
          {
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles // Include roles scope
        });
        identity.SetDestinations(GetDestinations);


        return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name or Claims.PreferredUsername:
                yield return Destinations.AccessToken;

                if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }

    return Results.BadRequest("The specified grant type is not supported.");
});

app.MapGet("/api/protected", (HttpContext httpContext) =>
{
    var xx = httpContext.User.Identities.First();


    return Results.Ok(new { message = "You have accessed a protected resource." });
}).RequireAuthorization(options =>
{
    options.RequireRole("admin");
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
