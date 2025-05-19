using DigitalWallets.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using DigitalWallets.Domain.Interfaces.Repositories;

namespace DigitalWallets.Application.Transactions.Queries
{
    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, IReadOnlyCollection<Transaction>>
    {
        private readonly ITransactionRepository _repository;
        private readonly ILogger<GetAllTransactionsQueryHandler> _logger;

        public GetAllTransactionsQueryHandler(ITransactionRepository repository, ILogger<GetAllTransactionsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<Transaction>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all transactions from repository");
            return await _repository.GetAllAsync(cancellationToken);
        }
    }
}
