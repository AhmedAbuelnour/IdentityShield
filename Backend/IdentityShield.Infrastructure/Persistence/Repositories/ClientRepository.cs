using IdentityShield.Application.Interfaces.Repositories;
using IdentityShield.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityShield.Infrastructure.Persistence.Repositories
{
    public class ClientRepository(ApplicationDbContext _dbContext) : IClientRepository
    {
        public Task<int> CreateAsync(Client client, CancellationToken cancellationToken)
        {
            _dbContext.Set<Client>().Add(client);

            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> IsDuplicatedAsync(Client client, CancellationToken cancellationToken)
        {
            return _dbContext.Set<Client>().Where(a => a.ClientId == client.ClientId).AnyAsync(cancellationToken);
        }
    }
}
