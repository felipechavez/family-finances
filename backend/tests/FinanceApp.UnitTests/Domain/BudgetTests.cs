namespace FinanceApp.UnitTests.Domain;
using FinanceApp.Domain.Entities;
using FluentAssertions;

public class BudgetTests
{
    [Fact]
    public void UpdateLimit_WithValidAmount_ShouldUpdate()
    {
        var budget = Budget.Create(Guid.NewGuid(), Guid.NewGuid(), 100_000m);
        budget.UpdateLimit(200_000m);
        budget.MonthlyLimit.Should().Be(200_000m);
    }

    [Fact]
    public void Create_WithNegativeLimit_ShouldThrow()
    {
        var act = () => Budget.Create(Guid.NewGuid(), Guid.NewGuid(), -1m);
        act.Should().Throw<ArgumentException>();
    }
}
