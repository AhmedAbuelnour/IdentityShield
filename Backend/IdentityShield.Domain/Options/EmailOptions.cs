namespace IdentityShield.Domain.Options
{
    public class EmailOptions
    {
        public string MailFrom { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Host { get; set; } = default!;
    }
}
