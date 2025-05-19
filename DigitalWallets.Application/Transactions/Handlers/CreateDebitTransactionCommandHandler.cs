using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;
using DigitalWallets.Domain.Entities;
public class CreateDebitTransactionCommandHandler : IRequestHandler<CreateDebitTransactionCommand, Transaction>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDebitTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Transaction> Handle(CreateDebitTransactionCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdWithUserAsync(request.WalletId, cancellationToken);
        if (wallet == null)
            throw new ArgumentException("Wallet not found.");

        if (wallet.Balance < request.Amount)
            throw new InvalidOperationException("Insufficient balance.");

        wallet.Debit(request.Amount);

        var transaction = Transaction.CreateDebit(
            amount: request.Amount,
            description: request.Description,
            senderId: wallet.Id
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(wallet, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return transaction;
    }

}