using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Enums;
using MediatR;

namespace DigitalWallets.Application.Transactions.Commands;

public record UpdateTransactionCommand(
    Guid Id,
    decimal Amount,
    string Description,
    DateTime Timestamp,
    TransactionStatus Status
) : IRequest<Transaction>;
