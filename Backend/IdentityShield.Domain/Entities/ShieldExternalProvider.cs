using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ShieldExternalProvider
    {
        [Key] public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(UserId))] public ShieldUser IdentityUser { get; set; } = default!;
    }
}
