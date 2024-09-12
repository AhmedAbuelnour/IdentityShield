using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class TokenClaim : EntityBase
    {
        public required string ClaimType { get; set; } // The claim type (e.g., "scope", "client-specific-permission") Role
        public required string ClaimValue { get; set; } // The value of the claim {{Role}}
        public bool IsDynamic { get; set; } // false
        public Guid ClientId { get; set; }
        [ForeignKey(nameof(ClientId))] public Client Client { get; set; } = default!;
    }
}
