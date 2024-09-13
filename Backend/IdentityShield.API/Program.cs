using IdentityShield.Application;
using IdentityShield.Application.UseCases.Clients.CreateClient.Commands;
using IdentityShield.Application.UseCases.Realms.CreateRealm.Commands;
using IdentityShield.Infrastructure;
using IdentityShield.Infrastructure.Persistence;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationDIContainer().AddInfrastructureDIContainer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/db", async (ApplicationDbContext _dbContext) =>
{
    await _dbContext.Database.EnsureCreatedAsync();
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

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
