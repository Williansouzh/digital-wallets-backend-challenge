using DigitalWallets.Domain.Entities;
using MediatR;

namespace DigitalWallets.Application.Transactions.Commands;

public record CreateDebitTransactionCommand(
    Guid WalletId,
    decimal Amount,
    string Description
) : IRequest<bool>;
