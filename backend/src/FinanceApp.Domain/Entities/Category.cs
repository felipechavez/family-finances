namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

public class Category : Entity
{
    public Guid? FamilyId { get; private set; }  // null = global
    public string Name { get; private set; } = default!;
    public TransactionType Type { get; private set; }

    private Category() { }

    public static Category Create(string name, TransactionType type, Guid? familyId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Category { Name = name, Type = type, FamilyId = familyId };
    }
}
