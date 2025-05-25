namespace IdentityShield.Contracts.PhoneNumberManagement
{
    public static class RequestPhoneNumberChange
    {
        public class Request
        {
            public string NewPhoneNumber { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class ConfirmPhoneNumberChange
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
