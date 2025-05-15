using DigitalWallets.Domain.Account;
using DigitalWallets.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DigitalWallets.Infra.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public string LastName { get;  set; }
    public string Phone { get;  set; }
    public Wallet Wallet { get; set; } = null!;
    public ApplicationUser() { }
    public ApplicationUser(AuthUser authUser)
    {
        Id = authUser.Id; 
        Name = authUser.Name;
        LastName = authUser.LastName;
        Phone = authUser.Phone;
    }
}
