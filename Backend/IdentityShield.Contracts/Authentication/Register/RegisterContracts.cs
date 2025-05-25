namespace IdentityShield.Contracts.Authentication.Register
{
    public static class RegisterByEmail
    {
        public class Request
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }

    public static class ConfirmEmailRegistration
    {
        public class Request
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }

    public static class RegisterByPhoneNumber
    {
        public class Request
        {
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }

    public static class ConfirmPhoneNumberRegistration
    {
        public class Request
        {
            public string PhoneNumber { get; set; }
            public string Token { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }

    public static class RegisterByProvider
    {
        public class Request
        {
            public string ProviderName { get; set; }
            public string ProviderValue { get; set; }
            public string RoleName { get; set; } = "User";
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
