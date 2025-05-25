namespace IdentityShield.Contracts.EmailManagement
{
    public class RequestEmailChangeRequest
    {
        public string NewEmail { get; set; }
    }

    public class ConfirmEmailChangeRequest
    {
        public string Token { get; set; }
    }

    public class EmailChangeResponse
    {
        public bool Success { get; set; }
    }
}
