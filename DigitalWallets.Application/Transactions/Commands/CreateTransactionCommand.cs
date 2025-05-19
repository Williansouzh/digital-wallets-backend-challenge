using DigitalWallets.Domain.Entities;
using MediatR;

namespace DigitalWallets.Application.Transactions.Commands;

public record CreateTransactionCommand(
    Guid SenderId,
    Guid RecipientId,
    decimal Amount,
    string Description
) : IRequest<Transaction>;
