using DigitalWallets.Application.Wallets.Commands;
using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using DigitalWallets.Domain.Interfaces.Services;
using MediatR;

namespace DigitalWallets.Application.Wallets.Handlers;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, bool>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var walletExists = await _walletRepository.ExistsForUserAsync(request.UserId, cancellationToken);
        if (walletExists)
            return false;
        
        var wallet = new Wallet(request.UserId, request.InitialBalance);
        await _walletRepository.AddAsync(wallet, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }
}
