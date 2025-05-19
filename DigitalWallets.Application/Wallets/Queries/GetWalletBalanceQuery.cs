using DigitalWallets.Domain.Interfaces.Repositories;
using MediatR;

namespace DigitalWallets.Application.Wallets.Queries;

public record GetWalletBalanceQuery(Guid UserId) : IRequest<decimal>;
