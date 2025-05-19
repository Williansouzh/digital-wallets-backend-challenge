using DigitalWallets.Domain.Entities;
using MediatR;

namespace DigitalWallets.Application.Transactions.Queries;

public record GetTransactionsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<Transaction>>;
