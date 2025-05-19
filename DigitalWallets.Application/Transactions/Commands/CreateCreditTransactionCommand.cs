using MediatR;

namespace DigitalWallets.Application.Transactions.Commands;

public record CreateCreditTransactionCommand(
    Guid WalletId,
    decimal Amount,
    string Description
) : IRequest<bool>;
