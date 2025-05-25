namespace IdentityShield.Contracts.PhoneNumberManagement
{
    public class RequestPhoneNumberChangeRequest
    {
        public string NewPhoneNumber { get; set; }
    }

    public class ConfirmPhoneNumberChangeRequest
    {
        public string Token { get; set; }
    }

    public class PhoneNumberChangeResponse
    {
        public bool Success { get; set; }
    }
}
