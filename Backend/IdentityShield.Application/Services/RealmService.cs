using ErrorOr;
using Flaminco.ManualMapper.Abstractions;
using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Application.Interfaces.Services;
using IdentityShield.Application.UseCases.Realms.CreateRealm.Commands;
using IdentityShield.Domain.Constants;
using IdentityShield.Domain.Entities;

namespace IdentityShield.Application.Services
{
    internal class RealmService(IRealmRepository _realmRepo, IMapper _mapper) : IRealmService
    {
        public async Task<ErrorOr<bool>> CreateAsync(CreateRealmRequest request, CancellationToken cancellationToken = default)
        {
            Realm realm = _mapper.Map<CreateRealmRequest, Realm>(request);

            if (await _realmRepo.IsDuplicatedAsync(realm, cancellationToken))
            {
                return Error.Validation("duplicated realm", "The system found an existed realm with the same name");
            }

            int effectedRows = await _realmRepo.CreateAsync(realm, cancellationToken);

            if (effectedRows == Constant.AppConstants.NoEffectedRows)
            {
                return Error.Validation("no effect", "The system didn't make the required change");
            }

            return true;
        }
    }
}
