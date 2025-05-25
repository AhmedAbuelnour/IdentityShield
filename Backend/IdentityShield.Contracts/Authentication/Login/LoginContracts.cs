namespace IdentityShield.Contracts.Authentication.Login
{
    public static class LoginByEmail
    {
        public class Request
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserId { get; set; }
            public bool IsEmailVerified { get; set; }
            public bool IsPhoneNumberVerified { get; set; }
        }
    }

    public static class LoginByPhoneNumber
    {
        public class Request
        {
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserId { get; set; }
            public bool IsEmailVerified { get; set; }
            public bool IsPhoneNumberVerified { get; set; }
        }
    }

    public static class LoginByProvider
    {
        public class Request
        {
            public string ProviderName { get; set; }
            public string ProviderValue { get; set; }
        }

        public class Response
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserId { get; set; }
            public bool IsEmailVerified { get; set; }
            public bool IsPhoneNumberVerified { get; set; }
        }
    }

    public static class LoginByManagedAccount
    {
        public class Request
        {
            public string ManagedAccountId { get; set; }
        }

        public class Response
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserId { get; set; }
            public bool IsEmailVerified { get; set; }
            public bool IsPhoneNumberVerified { get; set; }
        }
    }
}
