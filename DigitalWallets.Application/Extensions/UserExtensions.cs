using DigitalWallets.Domain.Account;
using DigitalWallets.Infra.Data.Identity;

namespace DigitalWallets.Application.Extensions;

public static class UserExtensions
{
    public static AuthUser ToAuthUser(this ApplicationUser user)
    {
        return new AuthUser
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Phone = user.Phone,
            Email = user.Email, 
            Roles = Enumerable.Empty<string>() 
        };
    }
}
