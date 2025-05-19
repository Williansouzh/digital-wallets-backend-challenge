using DigitalWallets.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalWallets.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identifier");
        }
        return userId;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetCurrentBalance()
    {
        try
        {
            var userId = GetCurrentUserId();
            var balance = await _walletService.GetBalanceAsync(userId);
            return Ok(new { Balance = balance });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(new { Error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wallet balance");
            return StatusCode(500, new { Error = "Could not retrieve balance" });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateWallet([FromBody] decimal initialBalance = 0)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _walletService.CreateWalletAsync(userId, initialBalance);
            return result ? Ok(new { Message = "Wallet created successfully" })
                         : BadRequest(new { Error = "Wallet already exists" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized wallet creation attempt");
            return Unauthorized(new { Error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wallet");
            return StatusCode(500, new { Error = "Could not create wallet" });
        }
    }

    [HttpPost("credit")]
    public async Task<IActionResult> CreditWallet([FromBody] decimal amount)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _walletService.CreditAsync(userId, amount);
            return result ? Ok(new { Message = "Wallet credited successfully" })
                         : BadRequest(new { Error = "Invalid credit operation" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid credit amount");
            return BadRequest(new { Error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized credit attempt");
            return Unauthorized(new { Error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crediting wallet");
            return StatusCode(500, new { Error = "Could not credit wallet" });
        }
    }

    [HttpPost("debit")]
    public async Task<IActionResult> DebitWallet([FromBody] decimal amount)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _walletService.DebitAsync(userId, amount);
            return result ? Ok(new { Message = "Wallet debited successfully" })
                         : BadRequest(new { Error = "Insufficient funds or invalid operation" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid debit amount");
            return BadRequest(new { Error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized debit attempt");
            return Unauthorized(new { Error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error debiting wallet");
            return StatusCode(500, new { Error = "Could not debit wallet" });
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferFunds(
        [FromQuery] Guid recipientId,
        [FromBody] decimal amount)
    {
        try
        {
            var senderId = GetCurrentUserId();
            var (success, newBalance) = await _walletService.TransferAsync(senderId, recipientId, amount);

            return success
                ? Ok(new
                {
                    Message = "Transfer completed successfully",
                    NewBalance = newBalance
                })
                : BadRequest(new
                {
                    Error = "Transfer failed",
                    CurrentBalance = newBalance
                });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid transfer parameters");
            return BadRequest(new { Error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized transfer attempt");
            return Unauthorized(new { Error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring funds");
            return StatusCode(500, new { Error = "Could not complete transfer" });
        }
    }

    [HttpGet("exists")]
    public async Task<IActionResult> CheckWalletExists()
    {
        try
        {
            var userId = GetCurrentUserId();
            var exists = await _walletService.ExistsForUserAsync(userId);
            return Ok(new { Exists = exists });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized existence check");
            return Unauthorized(new { Error = "Authentication required" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking wallet existence");
            return StatusCode(500, new { Error = "Could not check wallet status" });
        }
    }
}