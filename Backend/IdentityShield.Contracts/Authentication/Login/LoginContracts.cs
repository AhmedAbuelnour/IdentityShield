namespace IdentityShield.Contracts.Authentication.Login
{
    public class LoginByEmailRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginByPhoneNumberRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class LoginByProviderRequest
    {
        public string ProviderName { get; set; }
        public string ProviderValue { get; set; }
    }

    public class LoginByManagedAccountRequest
    {
        public string ManagedAccountId { get; set; }
    }

    public class LoginResponse
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
