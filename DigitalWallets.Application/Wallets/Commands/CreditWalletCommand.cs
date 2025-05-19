using MediatR;

namespace DigitalWallets.Application.Wallets.Commands;

public record CreditWalletCommand(Guid UserId, decimal Amount) : IRequest<bool>;
