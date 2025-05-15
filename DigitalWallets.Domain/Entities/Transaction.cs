using DigitalWallets.Domain.Account;
using System;

namespace DigitalWallets.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Guid SenderId { get; private set; }
    public AuthUser Sender { get; private set; } = null!;

    public Guid RecipientId { get; private set; }
    public AuthUser Recipient { get; private set; } = null!;

    protected Transaction() { }

    public Transaction(decimal amount, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        Id = Guid.NewGuid();
        ValidateDomain(amount, timestamp, senderId, sender, recipientId, recipient);
        ApplyState(amount, timestamp, senderId, sender, recipientId, recipient);
    }

    public Transaction(Guid id, decimal amount, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        ValidateDomain(amount, timestamp, senderId, sender, recipientId, recipient);
        Id = id;
        ApplyState(amount, timestamp, senderId, sender, recipientId, recipient);
    }

    public void Update(decimal amount, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        ValidateDomain(amount, timestamp, senderId, sender, recipientId, recipient);
        ApplyState(amount, timestamp, senderId, sender, recipientId, recipient);
    }

    private void ApplyState(decimal amount, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        Amount = amount;
        Timestamp = timestamp;
        SenderId = senderId;
        Sender = sender;
        RecipientId = recipientId;
        Recipient = recipient;
    }

    private void ValidateDomain(decimal amount, DateTime timestamp, Guid senderId, AuthUser sender, Guid recipientId, AuthUser recipient)
    {
        DomainExceptValidation.When(amount <= 0, "Transaction amount must be greater than zero.");
        DomainExceptValidation.When(senderId == Guid.Empty, "Sender ID cannot be empty.");
        DomainExceptValidation.When(recipientId == Guid.Empty, "Recipient ID cannot be empty.");
        DomainExceptValidation.When(senderId == recipientId, "Sender and recipient cannot be the same.");
        DomainExceptValidation.When(sender == null, "Sender cannot be null.");
        DomainExceptValidation.When(recipient == null, "Recipient cannot be null.");

        DomainExceptValidation.When(timestamp == default, "Timestamp must be valid.");
        DomainExceptValidation.When(timestamp > DateTime.UtcNow.AddDays(1), "Timestamp cannot be more than a day in the future.");
        DomainExceptValidation.When(timestamp < DateTime.UtcNow.AddYears(-1), "Timestamp cannot be more than a year in the past.");
    }
}
