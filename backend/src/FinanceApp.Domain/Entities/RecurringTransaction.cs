using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

[Table("recurring_transactions")]
public class RecurringTransaction : Entity
{
    [Column("family_id")]
    public Guid FamilyId { get; set; }

    [Column("template_account_id")]
    public Guid TemplateAccountId { get; set; }

    [Column("template_category_id")]
    public Guid TemplateCategoryId { get; set; }

    [Column("type")]
    public TransactionType Type { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("description")]
    public string Description { get; set; } = default!;

    [Column("recurrence_type")]
    public RecurrenceType RecurrenceType { get; set; }

    [Column("next_execution_date")]
    public DateOnly NextExecutionDate { get; set; }

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
