namespace IdentityShield.Domain.Options
{
    public class VodafoneSMSOptions
    {
        public string BaseUrl { get; set; } = default!;
        public string AccountId { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string SecureHashSecretKey { get; set; } = default!;
        public string SenderName { get; set; } = default!;
    }
}
