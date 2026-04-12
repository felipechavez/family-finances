using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Serialization;
namespace FinanceApp.Domain.Entities;

[Table("categories")]
public class Category : Entity
{
    /// <summary>
    /// Gets the identifier of the owning family, or <c>null</c> if this is a global category.
    /// </summary>
    [Column("family_id")]
    [JsonInclude]
    [JsonPropertyName("family_id")]
    public Guid? FamilyId { get; set; }

    /// <summary>Gets the display name of the category.</summary>
    [Column("name")]
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>Gets the transaction type this category applies to (Income or Expense).</summary>
    [Column("type")]
    [JsonInclude]
    [JsonPropertyName("type")]
    public TransactionType Type { get; set; }

    public static Category Create(string name, TransactionType type, Guid? familyId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Category { Name = name, Type = type, FamilyId = familyId };
    }
}
