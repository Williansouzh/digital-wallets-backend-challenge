using DigitalWallets.Application.Wallets.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Wallets.Handlers;

public class DebitWalletCommandHandler : IRequestHandler<DebitWalletCommand, bool>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DebitWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DebitWalletCommand request, CancellationToken cancellationToken)
    {
        var newBalance = await _walletRepository.UpdateBalanceAsync(request.UserId, -request.Amount, cancellationToken);
        if (!newBalance.HasValue)
            return false;
        await _unitOfWork.CommitAsync(cancellationToken);
        return newBalance.HasValue;
    }
}
