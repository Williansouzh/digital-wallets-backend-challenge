using DigitalWallets.Application.Wallets.Queries;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Wallets.Handlers;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, decimal>
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletBalanceQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<decimal> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        return await _walletRepository.GetBalanceAsync(request.UserId, cancellationToken);
    }
}