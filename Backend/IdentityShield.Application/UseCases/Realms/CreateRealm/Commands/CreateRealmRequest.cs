namespace IdentityShield.Application.UseCases.Realms.CreateRealm.Commands
{
    public class CreateRealmRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
