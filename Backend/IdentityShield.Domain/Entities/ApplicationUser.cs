using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid RealmId { get; set; }
        //   public Dictionary<string, string>? Attributes { get; set; }
        [ForeignKey(nameof(RealmId))] public Realm Realm { get; set; } = default!;// Each user belongs to a realm
                                                                                  //  public ICollection<UserGroup> UserGroups { get; set; } // Groups the user belongs to
    }
}
