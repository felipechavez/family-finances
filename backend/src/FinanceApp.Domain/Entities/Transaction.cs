using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;
using Supabase.Postgrest.Attributes;
using System.Text.Json.Serialization;

namespace FinanceApp.Domain.Entities;

[Table("transactions")]
public class Transaction : Entity
{
    [Column("family_id")]
    [JsonInclude]
    [JsonPropertyName("family_id")]
    public Guid FamilyId { get; set; }

    [Column("account_id")]
    [JsonInclude]
    [JsonPropertyName("account_id")]
    public Guid AccountId { get; set; }

    [Column("user_id")]
    [JsonInclude]
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    [Column("category_id")]
    [JsonInclude]
    [JsonPropertyName("category_id")]
    public Guid CategoryId { get; set; }

    [Column("type")]
    [JsonInclude]
    [JsonPropertyName("type")]
    public TransactionType Type { get; set; }

    [Column("amount")]
    [JsonInclude]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [Column("currency")]
    [JsonInclude]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "CLP";

    [Column("description")]
    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;

    [Column("transaction_date")]
    [JsonInclude]
    [JsonPropertyName("transaction_date")]
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
