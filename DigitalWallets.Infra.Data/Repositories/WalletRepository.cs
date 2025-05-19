using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using DigitalWallets.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallets.Infra.Data.Repositories;

public class WalletRepository : Repository<Wallet>, IWalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .AnyAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var wallet = await _context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        if (wallet is null)
            throw new InvalidOperationException("Wallet not found for the given user.");

        return wallet.Balance;
    }

    public async Task<Wallet?> GetByUserIdWithUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Wallet>> GetWalletsWithBalanceAboveAsync(
        decimal amount,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Wallets
            .Where(w => w.Balance > amount)
            .OrderBy(w => w.Balance)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<(bool Success, decimal NewSenderBalance)> TransferAsync(
        Guid senderUserId,
        Guid receiverUserId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be greater than zero.");

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get sender wallet with lock for update
            var senderWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == senderUserId, cancellationToken);

            if (senderWallet == null)
                throw new InvalidOperationException("Sender wallet not found.");

            // Get receiver wallet with lock for update
            var receiverWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == receiverUserId, cancellationToken);

            if (receiverWallet == null)
                throw new InvalidOperationException("Receiver wallet not found.");

            // Check sender balance
            if (senderWallet.Balance < amount)
                return (false, senderWallet.Balance);

            // Perform transfer
            senderWallet.Debit(amount);
            receiverWallet.Credit(amount);

            return (true, senderWallet.Balance);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<decimal?> UpdateBalanceAsync(
        Guid userId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        if (wallet == null)
            return null;

        if (amount > 0)
        {
            wallet.Credit(amount);
        }
        else if (amount < 0)
        {
            wallet.Debit(Math.Abs(amount));
        }

        return wallet.Balance;
    }
}