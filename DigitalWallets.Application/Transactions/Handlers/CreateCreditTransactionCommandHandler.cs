using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;
using DomainTransaction = DigitalWallets.Domain.Entities.Transaction;
public class CreateCreditTransactionCommandHandler : IRequestHandler<CreateCreditTransactionCommand, bool>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCreditTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateCreditTransactionCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdWithUserAsync(request.WalletId, cancellationToken);
        if (wallet == null)
            return false;

        wallet.Credit(request.Amount);

        var transaction = DomainTransaction.Create(
            request.Amount,
            request.Description,
            DateTime.UtcNow,
            Guid.Empty, // SenderId is empty for credit
            null,       // Sender is null
            wallet.UserId,
            wallet.User
        );

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(wallet, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}