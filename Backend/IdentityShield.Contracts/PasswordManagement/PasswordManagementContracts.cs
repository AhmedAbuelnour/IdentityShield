namespace IdentityShield.Contracts.PasswordManagement
{
    public class ForgotPasswordByEmailRequest
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordByPhoneNumberRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordByEmailRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordByPhoneNumberRequest
    {
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class PasswordResponse
    {
        public bool Success { get; set; }
    }
}
