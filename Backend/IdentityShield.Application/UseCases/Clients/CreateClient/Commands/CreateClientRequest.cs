
namespace IdentityShield.Application.UseCases.Clients.CreateClient.Commands
{
    public class CreateClientRequest
    {
        public required Guid RealmId { get; set; }
        public required string GrantType { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUri { get; set; }
        public required string ClientId { get; set; }
    }
}
