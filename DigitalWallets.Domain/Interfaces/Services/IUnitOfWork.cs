using DigitalWallets.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IWalletRepository WalletRepository { get; }
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
