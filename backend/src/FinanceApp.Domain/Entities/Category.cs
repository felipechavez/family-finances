using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

[Table("categories")]
public class Category : Entity
{
    /// <summary>
    /// Gets the identifier of the owning family, or <c>null</c> if this is a global category.
    /// </summary>
    [Column("family_id")]
    public Guid? FamilyId { get; set; }

    /// <summary>Gets the display name of the category.</summary>
    [Column("name")]
    public string Name { get; set; } = default!;

    /// <summary>Gets the transaction type this category applies to (Income or Expense).</summary>
    [Column("type")]
    public TransactionType Type { get; set; }

    public static Category Create(string name, TransactionType type, Guid? familyId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Category { Name = name, Type = type, FamilyId = familyId };
    }
}
