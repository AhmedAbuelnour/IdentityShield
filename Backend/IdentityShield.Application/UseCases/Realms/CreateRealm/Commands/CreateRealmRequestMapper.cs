using Flaminco.ManualMapper.Abstractions;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.UseCases.Realms.CreateRealm.Commands
{
    public class CreateRealmRequestMapper : IMapHandler<CreateRealmRequest, Realm>
    {
        public Realm Handler(CreateRealmRequest source)
        {
            return new Realm
            {
                Name = source.Name,
                Description = source.Description,
                IsActive = true,
            };
        }
    }
}
