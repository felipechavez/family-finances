namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

public class Account : Entity
{
    public Guid FamilyId { get; private set; }
    public string Name { get; private set; } = default!;
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }

    private Account() { }

    public static Account Create(Guid familyId, string name, AccountType type, decimal initialBalance = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Account { FamilyId = familyId, Name = name, Type = type, Balance = initialBalance };
    }
}
