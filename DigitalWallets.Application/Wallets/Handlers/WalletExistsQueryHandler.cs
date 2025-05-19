using DigitalWallets.Application.Wallets.Queries;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Wallets.Handlers;

public class WalletExistsQueryHandler : IRequestHandler<WalletExistsQuery, bool>
{
    private readonly IWalletRepository _walletRepository;

    public WalletExistsQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<bool> Handle(WalletExistsQuery request, CancellationToken cancellationToken)
    {
        return await _walletRepository.ExistsForUserAsync(request.UserId, cancellationToken);
    }
}
