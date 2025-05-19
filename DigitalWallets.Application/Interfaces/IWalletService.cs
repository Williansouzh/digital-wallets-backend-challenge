namespace DigitalWallets.Application.Interfaces;

public interface IWalletService
{
    Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CreateWalletAsync(Guid userId, decimal initialBalance = 0, CancellationToken cancellationToken = default);
    Task<bool> CreditAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default);
    Task<bool> DebitAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default);
    Task<(bool Success, decimal NewSenderBalance)> TransferAsync(
        Guid senderUserId,
        Guid receiverUserId,
        decimal amount,
        CancellationToken cancellationToken = default);
}
