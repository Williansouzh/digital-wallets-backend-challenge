using MediatR;

namespace DigitalWallets.Application.Transactions.Commands;

public record DeleteTransactionCommand(Guid Id) : IRequest<bool>;
