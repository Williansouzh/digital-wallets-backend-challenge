using DigitalWallets.Application.Transactions.Queries;
using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Transactions.Handlers;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Transaction>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Transaction> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken);
    }
}
