
namespace DigitalWallets.Infra.Data.Exceptions;

public class RoleNotFoundException : Exception
{
    public RoleNotFoundException(string roleName) : base($"Role '{roleName}' not found.")
    {
    }
}
