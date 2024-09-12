using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ClientPermission
    {
        public Guid ClientId { get; set; }
        public Guid PermissionId { get; set; }
        [ForeignKey(nameof(ClientId))] public Client Client { get; set; } = default!;
        [ForeignKey(nameof(ClientId))] public Permission Permission { get; set; } = default!;
    }

}
