using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class TokenTracker
    {
        [Key] public string Id { get; set; } = default!;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string Purpose { get; set; } = default!;
        public string UserId { get; set; } = default!;
        [ForeignKey(nameof(UserId))] public ApplicationUser IdentityUser { get; set; } = default!;
        public string Provider { get; set; }
    }
}
