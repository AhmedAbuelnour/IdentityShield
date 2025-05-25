using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ShieldRefreshToken
    {
        [Key] public string Token { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsActive => !RevokedAt.HasValue && !IsExpired;

        public string UserId { get; set; } = default!;

        public Guid? SessionId { get; set; }

        [ForeignKey(nameof(UserId))] public ShieldUser IdentityUser { get; set; } = default!;

    }
}
