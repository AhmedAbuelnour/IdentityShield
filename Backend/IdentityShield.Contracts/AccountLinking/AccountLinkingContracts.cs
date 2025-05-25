namespace IdentityShield.Contracts.AccountLinking
{
    public static class CreateManagedAccount
    {
        public class Request
        {
            public required string ManagerId { get; set; }
            public required string UserName { get; set; }
            public required string RoleName { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }

    }

    public static class LinkManagedAccount
    {
        public class Request
        {
            public required string ManagerId { get; set; }
            public required string ManagedId { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class UnlinkManagedAccount
    {
        public class Request
        {
            public required string ManagerId { get; set; }
            public required string ManagedId { get; set; }
        }

        public class Response
        {
            public bool Success { get; set; }
        }
    }

    public static class GetManagerAccounts
    {
        public class Request
        {
            public required string ManagerId { get; set; }
        }
        public class Response
        {
            public IEnumerable<ManagedAccountResponse> Accounts { get; set; }
        }

        public class ManagedAccountResponse
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
        }
    }

    public static class GetManagedAccounts
    {
        public class Request
        {
            public required string ManagedId { get; set; }
        }
        public class Response
        {
            public IEnumerable<ManagedAccountResponse> Accounts { get; set; }
        }

        public class ManagedAccountResponse
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
        }
    }
}
