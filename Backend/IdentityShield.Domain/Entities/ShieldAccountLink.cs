namespace IdentityShield.Domain.Entities
{
    public class ShieldAccountLink
    {
        public string ManagerAccountId { get; set; }
        public string ManagedAccountId { get; set; }
        public string LinkType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true; // Indicates if the relationship is active
        // Navigation properties
        public virtual ShieldUser ManagerAccount { get; set; } // Reference to manager
        public virtual ShieldUser ManagedAccount { get; set; } // Reference to managed account

    }
}
