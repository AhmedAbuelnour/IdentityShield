namespace IdentityShield.Contracts.Authentication.Logout
{
    public class LogoutRequest
    {
    }

    public class EnforceLogoutByEmailRequest
    {
        public string Email { get; set; }
    }

    public class EnforceLogoutByPhoneNumberRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class ConfirmEnforceLogoutRequest
    {
        public string Token { get; set; }
    }

    public class LogoutResponse
    {
        public bool Success { get; set; }
    }
}
