namespace IdentityShield.Domain.Entities
{
    public class Realm : EntityBase
    {
        public required string Name { get; set; } // Name of the realm
        public string? Description { get; set; } // Description of the realm
        public bool IsActive { get; set; } // Indicates if the realm is active
        public ICollection<ApplicationUser> Users { get; set; } // Users in the realm
        public ICollection<Client> Clients { get; set; } // Clients for the realm
        public ICollection<Group> Groups { get; set; } // Clients for the realm
        public ICollection<ApplicationRole> Roles { get; set; } // Roles available in the realm
    }
}
