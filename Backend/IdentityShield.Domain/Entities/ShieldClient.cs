using OpenIddict.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityShield.Domain.Entities
{
    public class ShieldClient : OpenIddictEntityFrameworkCoreApplication<Guid, ShieldAuthorization, ShieldToken>
    {
        public Guid? RealmId { get; set; }
        [ForeignKey(nameof(RealmId))] public Realm Realm { get; set; }
    }

    public class ShieldAuthorization : OpenIddictEntityFrameworkCoreAuthorization<Guid, ShieldClient, ShieldToken>
    {
    }

    public class ShieldScope : OpenIddictEntityFrameworkCoreScope<Guid>
    {
    }

    public class ShieldToken : OpenIddictEntityFrameworkCoreToken<Guid, ShieldClient, ShieldAuthorization>
    {
    }
}
