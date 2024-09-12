using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public Guid RealmId { get; set; }
        [ForeignKey(nameof(RealmId))] public Realm Realm { get; set; } = default!; // Each role belongs to a realm
        public ICollection<RolePermission> RolePermissions { get; set; } // Permissions assigned to the role
        public ICollection<RoleGroup> RoleGroups { get; set; } // Groups assigned to the role
    }
}
