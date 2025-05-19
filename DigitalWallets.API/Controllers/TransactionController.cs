using DigitalWallets.Application.Interfaces;
using DigitalWallets.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalWallets.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionService _transactionService;

    public TransactionController(
        ILogger<TransactionController> logger,
        ITransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);

        if (transaction is null)
        {
            return NotFound(new { Message = $"Transaction with ID {id} not found." });
        }

        return Ok(transaction);
    }
}
