using DigitalWallets.Domain.Entities;
using MediatR;

namespace DigitalWallets.Application.Transactions.Commands;

public record CreateTransferTransactionCommand(
    Guid SenderWalletId,
    Guid RecipientWalletId,
    decimal Amount,
    string Description
) : IRequest<Transaction>;
