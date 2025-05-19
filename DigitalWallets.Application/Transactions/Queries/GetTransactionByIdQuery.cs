using DigitalWallets.Domain.Entities;
using MediatR;

namespace DigitalWallets.Application.Transactions.Queries;

public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<Transaction>;
