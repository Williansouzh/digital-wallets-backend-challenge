using AutoMapper;
using DigitalWallets.Application.DTOs;
using DigitalWallets.Application.Interfaces;
using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Application.Transactions.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DigitalWallets.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IMapper mapper, ILogger<TransactionService> logger, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TransactionDTO> CreateCreditAsync(Guid walletId, decimal amount, string description, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating credit transaction for wallet {WalletId}", walletId);
        var command = new CreateCreditTransactionCommand(walletId, amount, description);
        var transaction = await _mediator.Send(command, cancellationToken);
        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<TransactionDTO> CreateDebitAsync(Guid walletId, decimal amount, string description, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating debit transaction for wallet {WalletId}", walletId);
        var command = new CreateDebitTransactionCommand(walletId, amount, description);
        var transaction = await _mediator.Send(command, cancellationToken);
        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<TransactionDTO> CreateTransferAsync(Guid senderId, Guid recipientId, decimal amount, string description, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating transfer from wallet {SenderId} to {RecipientId}", senderId, recipientId);
        var command = new CreateTransferTransactionCommand(senderId, recipientId, amount, description);
        var transaction = await _mediator.Send(command, cancellationToken);
        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating generic transaction from {SenderId} to {RecipientId}", transactionDto.SenderId, transactionDto.RecipientId);

        var command = new CreateTransactionCommand(
            transactionDto.SenderId,
            transactionDto.RecipientId,
            transactionDto.Amount,
            transactionDto.Description
        );

        var transaction = await _mediator.Send(command, cancellationToken);
        if (transaction == null)
        {
            _logger.LogWarning("Transaction creation failed");
            throw new ApplicationException("Transaction creation failed");
        }

        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<bool> DeleteTransactionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting transaction with ID: {TransactionId}", id);
        var command = new DeleteTransactionCommand(id);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TransactionDTO>> GetAllTransactionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all transactions");
        var query = new GetAllTransactionsQuery();
        var transactions = await _mediator.Send(query, cancellationToken);
        return _mapper.Map<IReadOnlyCollection<TransactionDTO>>(transactions);
    }

    public async Task<TransactionDTO> GetTransactionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving transaction by ID: {TransactionId}", id);
        var query = new GetTransactionByIdQuery(id);
        var transaction = await _mediator.Send(query, cancellationToken);
        return _mapper.Map<TransactionDTO>(transaction);
    }

    public async Task<TransactionDTO> UpdateTransactionAsync(Guid id, TransactionDTO transactionDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating transaction {TransactionId}", id);

        var command = new UpdateTransactionCommand(
            id,
            transactionDto.Amount,
            transactionDto.Description,
            transactionDto.Timestamp,
            transactionDto.Status
        );

        var transaction = await _mediator.Send(command, cancellationToken);
        if (transaction == null)
        {
            _logger.LogWarning("Transaction update failed for ID: {TransactionId}", id);
            throw new ApplicationException("Transaction update failed");
        }

        return _mapper.Map<TransactionDTO>(transaction);
    }
}
