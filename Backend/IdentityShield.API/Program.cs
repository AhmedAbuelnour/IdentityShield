using Flaminco.MinimalMediatR.Extensions;
using IdentityShield.Application;
using IdentityShield.Infrastructure;
using OpenIddict.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config =>
{
    config.CustomSchemaIds(type => type.FullName.Replace("+", "_"));

    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Selaheltelmeez API",
        Version = "v1"
    });


    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field (Add Space between Bearer and JWT Token)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      Array.Empty<string>()
                    }
                  });
});

builder.Services.AddModules<Program>();

builder.Services.AddApplicationDIContainer()
       .AddInfrastructureDIContainer(builder.Configuration);

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ShieldDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/connect/token");
        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();
        options.AcceptAnonymousClients();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapModules();

app.Run();

