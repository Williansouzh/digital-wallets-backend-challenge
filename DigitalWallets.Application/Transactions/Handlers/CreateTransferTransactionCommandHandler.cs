using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

public class CreateTransferTransactionCommandHandler : IRequestHandler<CreateTransferTransactionCommand, Transaction>
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

    public async Task<Transaction> Handle(CreateTransferTransactionCommand request, CancellationToken cancellationToken)
    {
        var senderWallet = await _walletRepository.GetByUserIdWithUserAsync(request.SenderWalletId, cancellationToken);
        if (senderWallet == null)
            throw new ArgumentException("Sender wallet not found.");

        var recipientWallet = await _walletRepository.GetByUserIdWithUserAsync(request.RecipientWalletId, cancellationToken);
        if (recipientWallet == null)
            throw new ArgumentException("Recipient wallet not found.");

        if (senderWallet.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance in sender wallet.");

        senderWallet.Debit(request.Amount);
        recipientWallet.Credit(request.Amount);

        var transaction = Transaction.CreateTransfer(
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
