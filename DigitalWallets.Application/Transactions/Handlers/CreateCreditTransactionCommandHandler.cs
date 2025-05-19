using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;
using DigitalWallets.Domain.Entities;
using DomainTransaction = DigitalWallets.Domain.Entities.Transaction;

public class CreateCreditTransactionCommandHandler : IRequestHandler<CreateCreditTransactionCommand, Transaction>
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

    public async Task<Transaction> Handle(CreateCreditTransactionCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdWithUserAsync(request.WalletId, cancellationToken);
        if (wallet == null)
            throw new ArgumentException("Wallet not found for the given user.");

        wallet.Credit(request.Amount);

        var transaction = DomainTransaction.CreateCredit(
            amount: request.Amount,
            description: request.Description,
            recipientId: wallet.Id
        );

        // 4. Persiste transação e carteira atualizada
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _walletRepository.UpdateAsync(wallet, cancellationToken);

        // 5. Efetiva a transação
        await _unitOfWork.CommitAsync(cancellationToken);

        return transaction;
    }
}
