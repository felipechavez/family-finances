namespace FinanceApp.Application.Features.Reports.GetMonthlySummary;
using MediatR;

/// <summary>Query to retrieve the income/expense summary for a specific month.</summary>
/// <param name="FamilyId">The family to generate the report for.</param>
/// <param name="Year">The calendar year (e.g., 2025).</param>
/// <param name="Month">The calendar month (1–12).</param>
public record GetMonthlySummaryQuery(Guid FamilyId, int Year, int Month) : IRequest<MonthlySummaryDto>;

/// <summary>Monthly financial summary for a family.</summary>
/// <param name="Year">The report year.</param>
/// <param name="Month">The report month (1–12).</param>
/// <param name="TotalIncome">The sum of all income transactions.</param>
/// <param name="TotalExpense">The sum of all expense transactions.</param>
/// <param name="Balance">Net balance: <c>TotalIncome - TotalExpense</c>.</param>
/// <param name="ExpensesByCategory">Expense breakdown grouped by category, ordered by total descending.</param>
public record MonthlySummaryDto(
    int Year, int Month,
    decimal TotalIncome, decimal TotalExpense, decimal Balance,
    IReadOnlyList<CategoryExpenseDto> ExpensesByCategory);

/// <summary>Expense total for a single category, with its associated budget limit.</summary>
/// <param name="CategoryId">The category's identifier.</param>
/// <param name="CategoryName">The category's display name.</param>
/// <param name="Total">The total amount spent in this category for the period.</param>
/// <param name="BudgetLimit">The configured monthly budget limit, or zero if no budget exists.</param>
public record CategoryExpenseDto(Guid CategoryId, string CategoryName, decimal Total, decimal BudgetLimit);
