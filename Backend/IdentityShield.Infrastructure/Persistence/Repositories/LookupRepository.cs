using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Persistence.Repositories
{
    public class LookupRepository(ApplicationDbContext _dbContext) : ILookupRepository
    {
        public Task<List<PermissionLookup>> GetPermissionLookupsAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<PermissionLookup>().ToListAsync(cancellationToken);
        }
    }
}
