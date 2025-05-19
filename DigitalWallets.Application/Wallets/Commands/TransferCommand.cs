using MediatR;

namespace DigitalWallets.Application.Wallets.Commands;

public record TransferCommand(Guid SenderUserId, Guid ReceiverUserId, decimal Amount)
: IRequest<(bool Success, decimal NewSenderBalance)>;
