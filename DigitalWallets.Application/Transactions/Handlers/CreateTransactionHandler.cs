using DigitalWallets.Domain.Entities;
using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using DigitalWallets.Infra.Data.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using DomainTransaction = DigitalWallets.Domain.Entities.Transaction;

namespace DigitalWallets.Application.Transactions.Handlers;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Transaction>
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

    public async Task<Transaction> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var senderUser = await _userManager.FindByIdAsync(request.SenderId.ToString());
        var recipientUser = await _userManager.FindByIdAsync(request.RecipientId.ToString());

        if (senderUser == null || recipientUser == null || senderUser.Id == recipientUser.Id)
            throw new ArgumentException("Invalid sender or recipient user.");

        var senderWallet = await _walletRepository.GetByUserIdWithUserAsync(request.SenderId, cancellationToken);
        var recipientWallet = await _walletRepository.GetByUserIdWithUserAsync(request.RecipientId, cancellationToken);

        if (senderWallet == null || recipientWallet == null)
            throw new ArgumentException("Sender or recipient wallet not found.");

        if (senderWallet.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance.");

        senderWallet.Debit(request.Amount);
        recipientWallet.Credit(request.Amount);

        var transaction = DomainTransaction.CreateTransfer(
            amount: request.Amount,
            description: request.Description,
            senderId: senderWallet.Id,
            recipientId: recipientWallet.Id
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(senderWallet, cancellationToken);
        await _walletRepository.UpdateAsync(recipientWallet, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return transaction;
    }


}
