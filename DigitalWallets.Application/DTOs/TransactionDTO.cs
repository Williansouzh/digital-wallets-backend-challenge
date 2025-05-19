using DigitalWallets.Domain.Account;
using DigitalWallets.Domain.Enums;

namespace DigitalWallets.Application.DTOs;

public class TransactionDTO
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Guid SenderId { get; private set; }
    public AuthUser Sender { get; private set; } = null!;
    public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;
    public Guid RecipientId { get; private set; }
    public AuthUser Recipient { get; private set; } = null!;
}
