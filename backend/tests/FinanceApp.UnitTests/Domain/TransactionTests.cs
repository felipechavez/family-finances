namespace FinanceApp.UnitTests.Domain;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using FluentAssertions;

public class TransactionTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var tx = Transaction.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            TransactionType.Expense, 100_000m, "Supermercado",
            DateOnly.FromDateTime(DateTime.Today));

        tx.Amount.Should().Be(100_000m);
        tx.Type.Should().Be(TransactionType.Expense);
        tx.Description.Should().Be("Supermercado");
        tx.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrow()
    {
        var act = () => Transaction.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            TransactionType.Expense, -500m, "Bad", DateOnly.FromDateTime(DateTime.Today));

        act.Should().Throw<ArgumentException>().WithMessage("*positive*");
    }
}
