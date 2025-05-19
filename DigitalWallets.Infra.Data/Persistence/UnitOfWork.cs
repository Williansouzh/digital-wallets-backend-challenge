using DigitalWallets.Domain.Interfaces.Repositories;
using DigitalWallets.Domain.Interfaces.Services;
using DigitalWallets.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallets.Infra.Data.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
    private readonly ApplicationDbContext _context;

    public IWalletRepository WalletRepository { get; }

    public UnitOfWork(ApplicationDbContext context, IWalletRepository walletRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        WalletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Log or handle DB-specific exceptions here
            throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
