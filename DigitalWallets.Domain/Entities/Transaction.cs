using DigitalWallets.Domain.Account;
using DigitalWallets.Domain.Enums;
namespace DigitalWallets.Domain.Entities;


public class Transaction : Entity
{
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public DateTime Timestamp { get; private set; }
    public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;

    public Guid SenderId { get; private set; }
    public Guid RecipientId { get; private set; }

    public AuthUser Sender { get; private set; }
    public AuthUser Recipient { get; private set; }

    protected Transaction() { }

    private Transaction(Guid id, decimal amount, string description, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient, TransactionStatus status)
    {
        Id = id;
        ValidateDomain(amount, description, timestamp, senderId, sender, recipientId, recipient);
        ApplyState(amount, description, timestamp, senderId, sender, recipientId, recipient, status);
    }

    public static Transaction Create(decimal amount, string description, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        return new Transaction(Guid.NewGuid(), amount, description, timestamp, senderId, sender, recipientId, recipient, TransactionStatus.Completed);
    }

    public void Update(decimal amount, string description, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient, TransactionStatus status)
    {
        ValidateDomain(amount, description, timestamp, senderId, sender, recipientId, recipient);
        ApplyState(amount, description, timestamp, senderId, sender, recipientId, recipient, status);
    }

    private void ApplyState(decimal amount, string description, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient, TransactionStatus status)
    {
        Amount = amount;
        Description = description;
        Timestamp = timestamp;
        SenderId = senderId;
        Sender = sender;
        RecipientId = recipientId;
        Recipient = recipient;
        Status = status;

    }

    private void ValidateDomain(decimal amount, string description, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        DomainExceptValidation.When(amount <= 0, "Transaction amount must be greater than zero.");
        DomainExceptValidation.When(string.IsNullOrWhiteSpace(description), "Transaction description is required.");
        DomainExceptValidation.When(timestamp == default, "Timestamp must be valid.");
        DomainExceptValidation.When(timestamp > DateTime.UtcNow.AddDays(1), "Timestamp cannot be more than one day in the future.");
        DomainExceptValidation.When(timestamp < DateTime.UtcNow.AddYears(-1), "Timestamp cannot be more than one year in the past.");

        DomainExceptValidation.When(senderId == Guid.Empty, "Sender ID cannot be empty.");
        DomainExceptValidation.When(recipientId == Guid.Empty, "Recipient ID cannot be empty.");
        DomainExceptValidation.When(senderId == recipientId, "Sender and recipient cannot be the same.");

        DomainExceptValidation.When(sender == null, "Sender cannot be null.");
        DomainExceptValidation.When(recipient == null, "Recipient cannot be null.");
    }
}
