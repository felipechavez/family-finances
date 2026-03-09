namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

public class Transaction : Entity
{
    public Guid FamilyId { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "CLP";
    public string Description { get; private set; } = default!;
    public DateOnly TransactionDate { get; private set; }

    private Transaction() { }

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
