namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

public class RecurringTransaction : Entity
{
    public Guid FamilyId { get; private set; }
    public Guid TemplateAccountId { get; private set; }
    public Guid TemplateCategoryId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = default!;
    public RecurrenceType RecurrenceType { get; private set; }
    public DateOnly NextExecutionDate { get; private set; }

    private RecurringTransaction() { }

    public static RecurringTransaction Create(
        Guid familyId, Guid accountId, Guid categoryId,
        TransactionType type, decimal amount, string description,
        RecurrenceType recurrenceType, DateOnly nextExecutionDate)
        => new()
        {
            FamilyId = familyId,
            TemplateAccountId = accountId,
            TemplateCategoryId = categoryId,
            Type = type,
            Amount = amount,
            Description = description,
            RecurrenceType = recurrenceType,
            NextExecutionDate = nextExecutionDate
        };
}
