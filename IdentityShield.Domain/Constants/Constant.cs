namespace IdentityShield.Domain.Constants
{
    public static class Constant
    {
        public static class TokenProviders
        {
            public const string OtpToken = "ShieldOTPTokenProvider";
        }

        public static class Purposes
        {
            public const string ForgotPassword = "ForgotPassword";
            public const string Register = "Register";
            public const string ChangeEmail = "ChangeEmail";
            public const string ChangePhoneNumber = "ChangePhoneNumber";
        }

        public static class TokenTrackerProvider
        {
            public const string Email = "Email";
            public const string PhoneNumber = "PhoneNumber";
        }
    }
}
