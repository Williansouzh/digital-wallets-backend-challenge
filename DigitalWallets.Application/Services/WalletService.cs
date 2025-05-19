using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DigitalWallets.Application.Interfaces;
using DigitalWallets.Application.Wallets.Commands;
using DigitalWallets.Application.Wallets.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallets.Application.Services;

public class WalletService : IWalletService
{
    private readonly IMediator _mediator;
    private readonly ILogger<WalletService> _logger;
    private readonly ITransactionService _transactionService;

    public WalletService(IMapper mapper, IMediator mediator, ILogger<WalletService> logger, ITransactionService transactionService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _transactionService = transactionService;
    }

    public async Task<bool> CreateWalletAsync(Guid userId, decimal initialBalance = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating wallet for user {UserId} with initial balance {InitialBalance}",
                userId, initialBalance);

            var command = new CreateWalletCommand(userId, initialBalance);
            ValidateCommand(command);

            return await _mediator.Send(command, cancellationToken);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed while creating wallet for user {UserId}", userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wallet for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> CreditAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Crediting {Amount} to wallet for user {UserId}", amount, userId);

            if (amount <= 0)
            {
                _logger.LogWarning("Invalid credit amount {Amount} for user {UserId}", amount, userId);
                throw new ArgumentException("Credit amount must be greater than zero");
            }

            var command = new CreditWalletCommand(userId, amount);
            ValidateCommand(command);
            var creditResult = await _mediator.Send(command, cancellationToken);
            if (creditResult)
            {
                // Criar transação de crédito
                var description = $"Credit of {amount} to wallet {userId}";
                await _transactionService.CreateCreditAsync(userId, amount, description, cancellationToken);
            }

            return creditResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crediting {Amount} to wallet for user {UserId}", amount, userId);
            throw;
        }
    }

    public async Task<bool> DebitAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Debiting {Amount} from wallet for user {UserId}", amount, userId);

            if (amount <= 0)
            {
                _logger.LogWarning("Invalid debit amount {Amount} for user {UserId}", amount, userId);
                throw new ArgumentException("Debit amount must be greater than zero");
            }

            var command = new DebitWalletCommand(userId, amount);
            ValidateCommand(command);

            return await _mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error debiting {Amount} from wallet for user {UserId}", amount, userId);
            throw;
        }
    }

    public async Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if wallet exists for user {UserId}", userId);
            return await _mediator.Send(new WalletExistsQuery(userId), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking wallet existence for user {UserId}", userId);
            throw;
        }
    }

    public async Task<decimal> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting balance for user {UserId}", userId);
            return await _mediator.Send(new GetWalletBalanceQuery(userId), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting balance for user {UserId}", userId);
            throw;
        }
    }

    public async Task<(bool Success, decimal NewSenderBalance)> TransferAsync(
        Guid senderUserId,
        Guid receiverUserId,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Transferring {Amount} from user {SenderUserId} to user {ReceiverUserId}",
                amount, senderUserId, receiverUserId);

            if (amount <= 0)
            {
                _logger.LogWarning("Invalid transfer amount {Amount}", amount);
                throw new ArgumentException("Transfer amount must be greater than zero");
            }

            if (senderUserId == receiverUserId)
            {
                _logger.LogWarning("Sender and receiver cannot be the same user");
                throw new ArgumentException("Cannot transfer to the same wallet");
            }

            var command = new TransferCommand(senderUserId, receiverUserId, amount);
            ValidateCommand(command);

            return await _mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring {Amount} from {SenderUserId} to {ReceiverUserId}",
                amount, senderUserId, receiverUserId);
            throw;
        }
    }

    private void ValidateCommand(object command)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(command, serviceProvider: null, items: null);

        if (!Validator.TryValidateObject(command, validationContext, validationResults, true))
        {
            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            var combinedErrors = string.Join(Environment.NewLine, errorMessages);

            _logger.LogError("Command validation failed: {Errors}", combinedErrors);
            throw new ValidationException($"Command validation failed: {combinedErrors}");
        }
    }
}