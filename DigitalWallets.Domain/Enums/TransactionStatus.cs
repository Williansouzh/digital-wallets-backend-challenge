
namespace DigitalWallets.Domain.Enums
{
    public enum TransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2
    }
    public enum TransactionType
    {
        Transfer,  // Between two wallets
        Credit,    // Money coming into the system
        Debit      // Money going out of the system
    }
}

