namespace IdentityShield.Contracts.ProviderManagement
{
    public class LinkProviderRequest
    {
        public string ProviderName { get; set; }
        public string ProviderValue { get; set; }
    }

    public class UnlinkProviderRequest
    {
        public string ProviderName { get; set; }
    }

    public class ProviderResponse
    {
        public bool Success { get; set; }
    }
}
