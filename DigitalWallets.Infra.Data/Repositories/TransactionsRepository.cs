using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using DigitalWallets.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallets.Infra.Data.Repositories;

public class TransactionsRepository : Repository<Transaction>, ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    public TransactionsRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(t => t.SenderId == userId || t.RecipientId == userId)
            .ToListAsync(cancellationToken);
    }
}
