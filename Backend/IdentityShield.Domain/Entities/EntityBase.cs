using System.ComponentModel.DataAnnotations;

namespace IdentityShield.Domain.Entities
{
    public class EntityBase
    {
        [Key] public Guid Id { get; set; }
    }
}
