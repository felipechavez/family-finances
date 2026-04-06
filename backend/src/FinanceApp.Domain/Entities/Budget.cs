using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;

[Table("budgets")]
public class Budget : Entity
{
    /// <summary>Gets the identifier of the family this budget belongs to.</summary>
    [Column("family_id")]
    public Guid FamilyId { get; set; }

    /// <summary>Gets the identifier of the expense category this budget applies to.</summary>
    [Column("category_id")]
    public Guid CategoryId { get; set; }

    /// <summary>Gets the maximum allowed spending for the category per month.</summary>
    [Column("monthly_limit")]
    public decimal MonthlyLimit { get; set; }

    private Budget() { }

    /// <summary>
    /// Creates a new <see cref="Budget"/> for the given family and category.
    /// </summary>
    /// <param name="familyId">The owning family's identifier.</param>
    /// <param name="categoryId">The category to budget for.</param>
    /// <param name="monthlyLimit">The monthly spending limit. Must be non-negative.</param>
    /// <returns>A new <see cref="Budget"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="monthlyLimit"/> is negative.</exception>
    public static Budget Create(Guid familyId, Guid categoryId, decimal monthlyLimit)
    {
        if (monthlyLimit < 0) throw new ArgumentException("Monthly limit cannot be negative", nameof(monthlyLimit));
        return new Budget { FamilyId = familyId, CategoryId = categoryId, MonthlyLimit = monthlyLimit };
    }

    /// <summary>
    /// Updates the monthly spending limit for this budget.
    /// </summary>
    /// <param name="newLimit">The new limit. Must be non-negative.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newLimit"/> is negative.</exception>
    public void UpdateLimit(decimal newLimit)
    {
        if (newLimit < 0) throw new ArgumentException("Monthly limit cannot be negative", nameof(newLimit));
        MonthlyLimit = newLimit;
    }
}
