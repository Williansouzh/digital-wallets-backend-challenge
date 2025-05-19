using MediatR;

namespace DigitalWallets.Application.Wallets.Commands;

public record DebitWalletCommand(Guid UserId, decimal Amount) : IRequest<bool>;
