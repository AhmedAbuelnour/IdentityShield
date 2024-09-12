using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class Client : EntityBase
    {
        public required string ClientId { get; set; } // Identifier for the client (e.g., "frontend-app")
        public string? ClientSecret { get; set; } // Secret key for confidential clients
        public required string GrantType { get; set; } // Grant type for the client (e.g., "password", "client_credentials")
        public string? RedirectUri { get; set; } // Redirect URI for OpenID Connect
        public Guid RealmId { get; set; }
        [ForeignKey(nameof(RealmId))] public Realm Realm { get; set; } = default!; // Each client belongs to a realm
        public ICollection<ClientPermission> ClientPermissions { get; set; } // Permissions assigned to the client
    }

}
