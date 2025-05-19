using DigitalWallets.Domain.Entities;
namespace DigitalWallets.Domain.Interfaces.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
