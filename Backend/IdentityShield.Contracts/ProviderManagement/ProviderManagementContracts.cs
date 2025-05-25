namespace IdentityShield.Contracts.ProviderManagement
{
    public static class LinkProvider
    {
        public class Request
        {
            public string ProviderName { get; set; }
            public string ProviderValue { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class UnlinkProvider
    {
        public class Request
        {
            public string ProviderName { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }
}
