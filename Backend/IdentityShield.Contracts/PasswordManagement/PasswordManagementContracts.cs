namespace IdentityShield.Contracts.PasswordManagement
{
    public static class ForgotPasswordByEmail
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

    public static class ForgotPasswordByPhoneNumber
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

    public static class ResetPasswordByEmail
    {
        public class Request
        {
            public string Email { get; set; }
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class ResetPasswordByPhoneNumber
    {
        public class Request
        {
            public string PhoneNumber { get; set; }
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class ChangePassword
    {
        public class Request
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }
}
