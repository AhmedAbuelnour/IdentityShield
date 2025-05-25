namespace IdentityShield.Contracts.Authentication.Logout
{
    public static class Logout
    {
        public class Request
        {
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class RequestEnforceLogoutByEmail
    {
        public class Request
        {
            public string Email { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class RequestEnforceLogoutByPhoneNumber
    {
        public class Request
        {
            public string PhoneNumber { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class ConfirmEnforceLogout
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
