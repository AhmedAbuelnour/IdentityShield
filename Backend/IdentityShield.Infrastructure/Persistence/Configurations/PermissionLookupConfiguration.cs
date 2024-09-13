using IdentityShield.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityShield.Infrastructure.Persistence.Configurations
{
    public class PermissionLookupConfiguration : IEntityTypeConfiguration<PermissionLookup>
    {
        public void Configure(EntityTypeBuilder<PermissionLookup> builder)
        {
            builder.Property(a => a.Id).HasDefaultValueSql("NEWID()");

            builder.HasData(
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Endpoint", Value = Permissions.Endpoints.Authorization, Description = "Allows the client to access the authorization endpoint to request authorization from the user." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Endpoint", Value = Permissions.Endpoints.Device, Description = "Allows the client to access the device endpoint, typically used in device flows." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Endpoint", Value = Permissions.Endpoints.Introspection, Description = "Allows the client to access the introspection endpoint to check the validity of a token." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Endpoint", Value = Permissions.Endpoints.Logout, Description = "Allows the client to access the logout endpoint to sign out a user." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Endpoint", Value = Permissions.Endpoints.Revocation, Description = "Allows the client to access the revocation endpoint to revoke tokens (access/refresh)." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Endpoint", Value = Permissions.Endpoints.Token, Description = "Allows the client to access the token endpoint to exchange authorization codes or refresh tokens for access tokens." },

                new PermissionLookup { Id = Guid.NewGuid(), Type = "GrantType", Value = Permissions.GrantTypes.AuthorizationCode, Description = "Allows the client to use the authorization code flow, where a user is redirected to the identity provider for authentication." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "GrantType", Value = Permissions.GrantTypes.ClientCredentials, Description = "Allows the client to authenticate and obtain an access token using its own credentials without user involvement." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "GrantType", Value = Permissions.GrantTypes.Implicit, Description = "Allows the client to use the implicit flow (deprecated)." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "GrantType", Value = Permissions.GrantTypes.Password, Description = "Allows the client to exchange a user's username and password for an access token (deprecated)." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "GrantType", Value = Permissions.GrantTypes.RefreshToken, Description = "Allows the client to use refresh tokens to obtain new access tokens when the old ones expire." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "GrantType", Value = Permissions.GrantTypes.DeviceCode, Description = "Allows the client to use the device code flow, typically for devices without input capabilities (TVs, IoT, etc.)." },

                new PermissionLookup { Id = Guid.NewGuid(), Type = "Scope", Value = Permissions.Scopes.Email, Description = "Allows the client to request access to the user's email address in the ID token or through the /userinfo endpoint." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Scope", Value = Permissions.Scopes.Profile, Description = "Allows the client to request access to the user's profile information in the ID token or through the /userinfo endpoint." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Scope", Value = Permissions.Scopes.Address, Description = "Allows the client to request access to the user's address." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Scope", Value = Permissions.Scopes.Phone, Description = "Allows the client to request access to the user's phone number." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Scope", Value = "scp:offline_access", Description = "Allows the client to request a refresh token to obtain new access tokens without user interaction." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "Scope", Value = Permissions.Scopes.Roles, Description = "Allows the client to request access to the user's roles." },

                new PermissionLookup { Id = Guid.NewGuid(), Type = "ResponseType", Value = Permissions.ResponseTypes.Code, Description = "Allows the client to request an authorization code in response." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "ResponseType", Value = Permissions.ResponseTypes.IdToken, Description = "Allows the client to request an ID token." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "ResponseType", Value = Permissions.ResponseTypes.IdTokenToken, Description = "Allows the client to request both an ID token and access token." },

                new PermissionLookup { Id = Guid.NewGuid(), Type = "ResponseMode", Value = "rm:query", Description = "Allows the client to request responses be returned via the query string." },
                new PermissionLookup { Id = Guid.NewGuid(), Type = "ResponseMode", Value = "rm:form_post", Description = "Allows the client to request responses be returned via form POST." }
            );
        }
    }
}
