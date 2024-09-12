using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Persistence.Repositories
{
    public class RealmRepository(ApplicationDbContext _dbContext) : IRealmRepository
    {
        public Task<int> CreateAsync(Realm realm, CancellationToken cancellationToken)
        {
            _dbContext.Set<Realm>().Add(realm);

            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> IsDuplicatedAsync(Realm realm, CancellationToken cancellationToken)
        {
            return _dbContext.Set<Realm>().Where(a => a.Name == realm.Name).AnyAsync(cancellationToken);
        }

    }
}
