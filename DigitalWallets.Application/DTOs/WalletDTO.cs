using DigitalWallets.Domain.Account;

namespace DigitalWallets.Application.DTOs;

public class WalletDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public AuthUser User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
