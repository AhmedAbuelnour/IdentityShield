using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class Group : EntityBase
    {
        public required string Name { get; set; } // Group name
        public string? Description { get; set; } // Group description
        public Guid RealmId { get; set; }
        [ForeignKey(nameof(RealmId))] public Realm Realm { get; set; } = default!; // Each group belongs to a realm
        public ICollection<UserGroup> UserGroups { get; set; } // Users in the group
        public ICollection<RoleGroup> RoleGroups { get; set; } // Roles assigned to the group
    }
}
