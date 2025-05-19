using MediatR;

namespace DigitalWallets.Application.Wallets.Queries;

public record WalletExistsQuery(Guid UserId) : IRequest<bool>;
