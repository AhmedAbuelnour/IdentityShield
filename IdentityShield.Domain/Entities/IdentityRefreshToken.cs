using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class IdentityRefreshToken
    {
        [Key] public string Id { get; set; } = default!;

        public string Token { get; set; } = default!;

        public DateTime ExpiresOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

        public bool IsActive => !RevokedOn.HasValue && !IsExpired;

        public string UserId { get; set; } = default!;

        [ForeignKey(nameof(UserId))] public ApplicationUser IdentityUser { get; set; } = default!;

    }
}
