namespace IdentityShield.Contracts.EmailManagement
{
    public static class RequestEmailChange
    {
        public class Request
        {
            public string NewEmail { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class ConfirmEmailChange
    {
        public class Request
        {
            public string Token { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }
}
