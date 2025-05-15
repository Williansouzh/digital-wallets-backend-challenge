using DigitalWallets.Domain.Account;

namespace DigitalWallets.Domain.Entities;

public class Wallet
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }
    public Guid UserId { get; private set; }

    protected Wallet() { }

    public Wallet(Guid userId, decimal initialBalance = 0)
    {
        Id = Guid.NewGuid();
        ValidateDomain(userId, initialBalance);
        ApplyState(userId, initialBalance);
    }

    private void ValidateDomain(Guid userId, decimal initialBalance)
    {
        DomainExceptValidation.When(userId == Guid.Empty, "User ID cannot be empty.");
        DomainExceptValidation.When(initialBalance < 0, "Initial balance cannot be negative.");
    }

    private void ApplyState(Guid userId, decimal initialBalance)
    {
        UserId = userId;
        Balance = initialBalance;
    }

    public void Credit(decimal amount)
    {
        DomainExceptValidation.When(amount <= 0, "Credit amount must be greater than zero.");
        Balance += amount;
    }

    public void Debit(decimal amount)
    {
        DomainExceptValidation.When(amount <= 0, "Debit amount must be greater than zero.");
        DomainExceptValidation.When(Balance < amount, "Insufficient funds.");
        Balance -= amount;
    }
}
