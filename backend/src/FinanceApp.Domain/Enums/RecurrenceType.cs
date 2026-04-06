namespace FinanceApp.Domain.Enums;

/// <summary>Defines how frequently a <see cref="FinanceApp.Domain.Entities.RecurringTransaction"/> repeats.</summary>
public enum RecurrenceType
{
    /// <summary>Repeats every day.</summary>
    Daily,
    /// <summary>Repeats every week.</summary>
    Weekly,
    /// <summary>Repeats every calendar month.</summary>
    Monthly,
    /// <summary>Repeats every year.</summary>
    Yearly
}
