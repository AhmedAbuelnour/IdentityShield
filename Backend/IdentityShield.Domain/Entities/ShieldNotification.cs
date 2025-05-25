using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ShieldNotification
    {
        [Key] public string Id { get; set; } = default!;

        public string UserId { get; set; } // 

        public string Recipient { get; set; } // the recipient of the notification ( the email, phone number, etc.)

        public string Provider { get; set; } // Email, SMS, etc.

        public string Purpose { get; set; } // the purpose of the notification ( Mobile-Verification, Email-Verification, etc.)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))] public ShieldUser IdentityUser { get; set; } = default!;
    }
}
