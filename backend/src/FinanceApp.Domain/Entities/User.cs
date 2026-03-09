namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;

public class User : Entity
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;

    private User() { }

    public static User Create(string name, string email, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        return new User { Name = name, Email = email, PasswordHash = passwordHash };
    }
}
