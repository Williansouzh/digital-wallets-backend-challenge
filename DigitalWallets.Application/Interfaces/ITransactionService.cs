using DigitalWallets.Application.DTOs;

namespace DigitalWallets.Application.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionDTO>> GetAllTransactionsAsync(CancellationToken cancellationToken = default);
    Task<TransactionDTO> GetTransactionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDto, CancellationToken cancellationToken = default);
    Task<TransactionDTO> UpdateTransactionAsync(Guid id, TransactionDTO transactionDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTransactionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TransactionDTO> CreateCreditAsync(Guid walletId, decimal amount, string description, CancellationToken cancellationToken = default);
    Task<TransactionDTO> CreateDebitAsync(Guid walletId, decimal amount, string description, CancellationToken cancellationToken = default);
    Task<TransactionDTO> CreateTransferAsync(Guid senderId, Guid recipientId, decimal amount, string description, CancellationToken cancellationToken = default);
}
