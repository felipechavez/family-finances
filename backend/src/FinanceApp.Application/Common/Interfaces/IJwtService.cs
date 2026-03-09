namespace FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;

public interface IJwtService
{
    string GenerateToken(User user, Guid? familyId = null, string? role = null);
}
