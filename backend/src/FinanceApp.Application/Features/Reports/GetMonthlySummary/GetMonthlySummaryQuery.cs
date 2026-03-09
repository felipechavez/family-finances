namespace FinanceApp.Application.Features.Reports.GetMonthlySummary;
using MediatR;

public record GetMonthlySummaryQuery(Guid FamilyId, int Year, int Month) : IRequest<MonthlySummaryDto>;
public record MonthlySummaryDto(
    int Year, int Month,
    decimal TotalIncome, decimal TotalExpense, decimal Balance,
    IReadOnlyList<CategoryExpenseDto> ExpensesByCategory);
public record CategoryExpenseDto(Guid CategoryId, string CategoryName, decimal Total, decimal BudgetLimit);
