namespace DigitalWallets.Domain.Account;

public class AuthUser
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public bool Success { get; set; } = true;
    public string Email { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();

    public AuthUser(Guid id, string name, string lastName, string phone, string email)
    {
        Id = id;
        Email = email;
        Name = name;
        LastName = lastName;
        Phone = phone;
        Success = true;
    }
}

