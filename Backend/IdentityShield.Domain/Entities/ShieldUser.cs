using Microsoft.AspNetCore.Identity;

namespace IdentityShield.Domain.Entities
{
    public class ShieldUser : IdentityUser
    {
        public bool? IsEnabled { get; set; }

        public Dictionary<string, object>? Attributes { get; set; }


        // Navigation properties
        public virtual ICollection<ShieldAccountLink> ManagingAccounts { get; set; } // Accounts this user manages
        public virtual ICollection<ShieldAccountLink> ManagedByAccounts { get; set; } // Accounts managing this user

    }
}
