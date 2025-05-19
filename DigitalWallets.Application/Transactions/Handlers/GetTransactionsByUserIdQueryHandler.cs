using DigitalWallets.Application.Transactions.Queries;
using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Transactions.Handlers;

public class GetTransactionsByUserIdQueryHandler : IRequestHandler<GetTransactionsByUserIdQuery, IEnumerable<Transaction>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsByUserIdQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<Transaction>> Handle(GetTransactionsByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _transactionRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);
    }
}
