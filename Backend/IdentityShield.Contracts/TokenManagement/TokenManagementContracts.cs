namespace IdentityShield.Contracts.TokenManagement
{
    public static class RefreshToken
    {
        public class Request
        {
            public string RefreshToken { get; set; }
        }

        public class Response
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
