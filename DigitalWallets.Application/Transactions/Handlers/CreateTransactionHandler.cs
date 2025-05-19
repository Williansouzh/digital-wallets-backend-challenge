using DigitalWallets.Application.Extensions;
using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using DigitalWallets.Infra.Data.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DomainTransaction = DigitalWallets.Domain.Entities.Transaction;

namespace DigitalWallets.Application.Transactions.Handlers;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, bool>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateTransactionCommandHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<bool> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var senderUser = await _userManager.FindByIdAsync(request.SenderId.ToString());
        var recipientUser = await _userManager.FindByIdAsync(request.RecipientId.ToString());

        if (senderUser == null || recipientUser == null || senderUser.Id == recipientUser.Id)
            return false;

        var senderWallet = await _walletRepository.GetByUserIdWithUserAsync(request.SenderId, cancellationToken);
        var recipientWallet = await _walletRepository.GetByUserIdWithUserAsync(request.RecipientId, cancellationToken);

        if (senderWallet == null || recipientWallet == null)
            return false;

        if (senderWallet.Balance < request.Amount)
            return false;

        senderWallet.Debit(request.Amount);
        recipientWallet.Credit(request.Amount);

        var transaction = DomainTransaction.Create(
            request.Amount,
            request.Description,
            DateTime.UtcNow,
            request.SenderId,
            senderUser.ToAuthUser(),
            request.RecipientId,
            recipientUser.ToAuthUser()
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(senderWallet, cancellationToken);
        await _walletRepository.UpdateAsync(recipientWallet, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);
        return true;
    }

}
