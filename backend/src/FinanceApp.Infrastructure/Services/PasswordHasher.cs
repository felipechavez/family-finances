namespace FinanceApp.Infrastructure.Services;
using FinanceApp.Application.Common.Interfaces;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(hash)) return false;      // missing hash => invalid credentials
        if (string.IsNullOrEmpty(password)) return false;      // defensive: validator normalmente evita esto

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception)
        {
            // Hash malformed / verification error: treat as invalid credentials
            return false;
        }
    }
}
