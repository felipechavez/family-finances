using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

[Table("transactions")]
public class Transaction : Entity
{
    [Column("family_id")]
    public Guid FamilyId { get; set; }

    [Column("account_id")]
    public Guid AccountId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("type")]
    public TransactionType Type { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("currency")]
    public string Currency { get; set; } = "CLP";

    [Column("description")]
    public string Description { get; set; } = default!;

    [Column("transaction_date")]
    public DateOnly TransactionDate { get; set; }

    public static Transaction Create(
        Guid familyId, Guid accountId, Guid userId, Guid categoryId,
        TransactionType type, decimal amount, string description,
        DateOnly transactionDate, string currency = "CLP")
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
        return new Transaction
        {
            FamilyId = familyId,
            AccountId = accountId,
            UserId = userId,
            CategoryId = categoryId,
            Type = type,
            Amount = amount,
            Currency = currency,
            Description = description,
            TransactionDate = transactionDate
        };
    }
}
