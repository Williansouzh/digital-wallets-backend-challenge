using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;
using DomainTransaction = DigitalWallets.Domain.Entities.Transaction;
public class CreateDebitTransactionCommandHandler : IRequestHandler<CreateDebitTransactionCommand, bool>
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

    public async Task<bool> Handle(CreateDebitTransactionCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdWithUserAsync(request.WalletId, cancellationToken);
        if (wallet == null)
            return false;

        if (wallet.Balance < request.Amount)
            return false;

        wallet.Debit(request.Amount);

        var transaction = DomainTransaction.Create(
            request.Amount,
            request.Description,
            DateTime.UtcNow,
            wallet.UserId,
            wallet.User,
            Guid.Empty, // No recipient for debit
            null       // No recipient user for debit
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(wallet, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}