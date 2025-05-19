using DigitalWallets.Domain.Entities;

namespace DigitalWallets.Domain.Interfaces.Repositories;

public interface IWalletRepository : IRepository<Wallet>
{
    Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByUserIdWithUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<decimal?> UpdateBalanceAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default);
    Task<(bool Success, decimal NewSenderBalance)> TransferAsync(
        Guid senderUserId,
        Guid receiverUserId,
        decimal amount,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Wallet>> GetWalletsWithBalanceAboveAsync(
        decimal amount,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
