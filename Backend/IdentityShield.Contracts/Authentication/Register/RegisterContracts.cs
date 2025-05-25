namespace IdentityShield.Contracts.Authentication.Register
{
    public class RegisterByEmailRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterByPhoneNumberRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class RegisterByProviderRequest
    {
        public string ProviderName { get; set; }
        public string ProviderValue { get; set; }
        public string RoleName { get; set; } = "User";
    }

    public class ConfirmEmailRegistrationRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class ConfirmPhoneNumberRegistrationRequest
    {
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
