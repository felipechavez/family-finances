namespace FinanceApp.Application.Common.Email;

/// <summary>Data required to render the daily summary email.</summary>
public record DailySummaryData(
    string FamilyName,
    DateOnly Date,
    decimal TotalIncome,
    decimal TotalExpenses,
    IReadOnlyList<CategorySummaryRow> Categories
);

/// <summary>Per-category row in the daily summary.</summary>
public record CategorySummaryRow(
    string CategoryName,
    decimal Amount,
    decimal MonthlyLimit   // 0 = no limit set
);
