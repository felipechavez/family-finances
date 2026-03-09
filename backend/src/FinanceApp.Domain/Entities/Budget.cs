namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;

public class Budget : Entity
{
    public Guid FamilyId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal MonthlyLimit { get; private set; }

    private Budget() { }

    public static Budget Create(Guid familyId, Guid categoryId, decimal monthlyLimit)
    {
        if (monthlyLimit < 0) throw new ArgumentException("Monthly limit cannot be negative", nameof(monthlyLimit));
        return new Budget { FamilyId = familyId, CategoryId = categoryId, MonthlyLimit = monthlyLimit };
    }

    public void UpdateLimit(decimal newLimit)
    {
        if (newLimit < 0) throw new ArgumentException("Monthly limit cannot be negative", nameof(newLimit));
        MonthlyLimit = newLimit;
    }
}
