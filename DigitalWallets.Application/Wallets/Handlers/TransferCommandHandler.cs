using DigitalWallets.Application.Wallets.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Wallets.Handlers;

public class TransferCommandHandler : IRequestHandler<TransferCommand, (bool Success, decimal NewSenderBalance)>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, decimal NewSenderBalance)> Handle(TransferCommand request, CancellationToken cancellationToken)
    {

        return await _walletRepository.TransferAsync(
            request.SenderUserId,
            request.ReceiverUserId,
            request.Amount,
            cancellationToken);

    }
}
