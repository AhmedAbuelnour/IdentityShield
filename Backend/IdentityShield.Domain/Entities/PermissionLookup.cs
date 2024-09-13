namespace IdentityShield.Domain.Entities
{
    public class PermissionLookup : EntityBase
    {
        public string Type { get; set; } // E.g., "Endpoint", "GrantType", "Scope", "Token", "ResponseType", "ResponseMode"
        public string Value { get; set; } // E.g., "Endpoints.Authorization", "GrantTypes.AuthorizationCode", etc.
        public string Description { get; set; } // Meaning of the permission
    }
}
