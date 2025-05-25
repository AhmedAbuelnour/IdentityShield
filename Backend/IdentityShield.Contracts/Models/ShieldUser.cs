namespace IdentityShield.Contracts.Models
{
    public class ShieldUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool IsEnabled { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }
}
