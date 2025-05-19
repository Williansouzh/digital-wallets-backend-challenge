using MediatR;

namespace DigitalWallets.Application.Wallets.Commands;

public record CreateWalletCommand(Guid UserId, decimal InitialBalance = 0) : IRequest<bool>;

