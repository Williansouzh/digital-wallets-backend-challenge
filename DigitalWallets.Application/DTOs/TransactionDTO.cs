using DigitalWallets.Domain.Account;
using DigitalWallets.Domain.Enums;

namespace DigitalWallets.Application.DTOs;

public class TransactionDTO
{
    public Guid Id { get;  set; }
    public decimal Amount { get;  set; }
    public DateTime Timestamp { get;  set; }
    public string Description { get;  set; } = string.Empty;
    public Guid SenderId { get;  set; }
    public TransactionStatus Status { get;  set; } = TransactionStatus.Pending;
    public Guid RecipientId { get;  set; }
}
