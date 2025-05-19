using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

using DomainTransaction = DigitalWallets.Domain.Entities.Transaction;
public class CreateTransferTransactionCommandHandler : IRequestHandler<CreateTransferTransactionCommand, bool>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransferTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateTransferTransactionCommand request, CancellationToken cancellationToken)
    {
        var senderWallet = await _walletRepository.GetByUserIdWithUserAsync(request.SenderWalletId, cancellationToken);
        if (senderWallet == null)
            return false;

        var recipientWallet = await _walletRepository.GetByUserIdWithUserAsync(request.RecipientWalletId, cancellationToken);
        if (recipientWallet == null)
            return false;

        if (senderWallet.Balance < request.Amount)
            return false;

        senderWallet.Debit(request.Amount);
        recipientWallet.Credit(request.Amount);

        var transaction = DomainTransaction.Create(
            request.Amount,
            request.Description,
            DateTime.UtcNow,
            senderWallet.UserId,
            senderWallet.User,
            recipientWallet.UserId,
            recipientWallet.User
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(senderWallet, cancellationToken);
        await _walletRepository.UpdateAsync(recipientWallet, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}