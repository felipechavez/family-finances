using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

[Table("accounts")]
public class Account : Entity
{
    /// <summary>Gets the identifier of the family that owns this account.</summary>
    [Column("family_id")]
    public Guid FamilyId { get; set; }

    /// <summary>Gets the display name of the account.</summary>
    [Column("name")]
    public string Name { get; set; } = default!;

    /// <summary>Gets the type of account (e.g., Cash, Bank, Savings).</summary>
    [Column("type")]
    public AccountType Type { get; set; }

    /// <summary>Gets the current balance of the account.</summary>
    [Column("balance")]
    public decimal Balance { get; set; }

    public static Account Create(Guid familyId, string name, AccountType type, decimal initialBalance = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Account { FamilyId = familyId, Name = name, Type = type, Balance = initialBalance };
    }
}
