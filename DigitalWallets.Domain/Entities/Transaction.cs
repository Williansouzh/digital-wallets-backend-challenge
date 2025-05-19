using DigitalWallets.Domain.Enums;

namespace DigitalWallets.Domain.Entities;

public class Transaction : Entity
{
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public DateTime Timestamp { get; private set; }
    public TransactionStatus Status { get; private set; }
    public TransactionType Type { get; private set; }

    public Guid? SenderId { get; private set; }
    public Guid? RecipientId { get; private set; }

    // Private constructor for EF Core
    protected Transaction() { }

    private Transaction(
        Guid id,
        decimal amount,
        string description,
        DateTime timestamp,
        Guid senderId,
        Guid recipientId,
        TransactionStatus status,
        TransactionType type)
    {
        Id = id;
        Amount = amount;
        Description = description;
        Timestamp = timestamp;
        SenderId = senderId;
        RecipientId = recipientId;
        Status = status;
        Type = type;

        Validate();
    }

    #region Factory Methods
    public static Transaction CreateTransfer(
        decimal amount,
        string description,
        Guid senderId,
        Guid recipientId)
    {
        return new Transaction(
            id: Guid.NewGuid(),
            amount: amount,
            description: description,
            timestamp: DateTime.UtcNow,
            senderId: senderId,
            recipientId: recipientId,
            status: TransactionStatus.Completed,
            type: TransactionType.Transfer);
    }

    public static Transaction CreateCredit(
        decimal amount,
        string description,
        Guid recipientId)
    {
        return new Transaction(
            id: Guid.NewGuid(),
            amount: amount,
            description: description,
            timestamp: DateTime.UtcNow,
            senderId: Guid.Empty, // No sender for credits
            recipientId: recipientId,
            status: TransactionStatus.Completed,
            type: TransactionType.Credit);
    }

    public static Transaction CreateDebit(
        decimal amount,
        string description,
        Guid senderId)
    {
        return new Transaction(
            id: Guid.NewGuid(),
            amount: amount,
            description: description,
            timestamp: DateTime.UtcNow,
            senderId: senderId,
            recipientId: Guid.Empty, // No recipient for debits
            status: TransactionStatus.Completed,
            type: TransactionType.Debit);
    }
    #endregion

    #region Behavior Methods
    public void MarkAsCompleted()
    {
        Status = TransactionStatus.Completed;
    }

    public void MarkAsFailed()
    {
        Status = TransactionStatus.Failed;
    }

    public void UpdateDescription(string newDescription)
    {
        DomainExceptValidation.When(string.IsNullOrWhiteSpace(newDescription),
            "Description cannot be empty");
        Description = newDescription;
    }
    #endregion

    #region Validation
    private void Validate()
    {
        ValidateCommonRules();
        ValidateTypeSpecificRules();
    }

    private void ValidateCommonRules()
    {
        DomainExceptValidation.When(Amount <= 0,
            "Amount must be greater than zero");
        DomainExceptValidation.When(string.IsNullOrWhiteSpace(Description),
            "Description is required");
        DomainExceptValidation.When(Timestamp == default,
            "Timestamp must be valid");
        DomainExceptValidation.When(Timestamp > DateTime.UtcNow.AddDays(1),
            "Timestamp cannot be more than one day in the future");
        DomainExceptValidation.When(Timestamp < DateTime.UtcNow.AddYears(-1),
            "Timestamp cannot be more than one year in the past");
    }

    private void ValidateTypeSpecificRules()
    {
        switch (Type)
        {
            case TransactionType.Transfer:
                ValidateTransferRules();
                break;

            case TransactionType.Credit:
                ValidateCreditRules();
                break;

            case TransactionType.Debit:
                ValidateDebitRules();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Type), "Invalid transaction type");
        }
    }

    private void ValidateTransferRules()
    {
        DomainExceptValidation.When(SenderId == Guid.Empty,
            "Sender ID is required for transfers");
        DomainExceptValidation.When(RecipientId == Guid.Empty,
            "Recipient ID is required for transfers");
        DomainExceptValidation.When(SenderId == RecipientId,
            "Sender and recipient cannot be the same for transfers");
    }

    private void ValidateCreditRules()
    {
        DomainExceptValidation.When(RecipientId == Guid.Empty,
            "Recipient ID is required for credits");
    }

    private void ValidateDebitRules()
    {
        DomainExceptValidation.When(SenderId == Guid.Empty,
            "Sender ID is required for debits");
    }
    #endregion
}