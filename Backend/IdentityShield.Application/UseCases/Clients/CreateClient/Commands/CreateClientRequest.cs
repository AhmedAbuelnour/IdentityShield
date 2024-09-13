
namespace IdentityShield.Application.UseCases.Clients.CreateClient.Commands
{
    public class CreateClientRequest
    {
        public required Guid RealmId { get; set; }
        public required string ClientId { get; set; }
        public required Guid[] Permissions { get; set; }
        public string? ClientSecret { get; set; }
        public string? DisplayName { get; set; }
        public string[]? RedirectUris { get; set; }
    }
}
