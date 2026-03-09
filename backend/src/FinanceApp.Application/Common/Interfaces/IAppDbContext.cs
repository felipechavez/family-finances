namespace FinanceApp.Application.Common.Interfaces;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Family> Families { get; }
    DbSet<FamilyMember> FamilyMembers { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<RecurringTransaction> RecurringTransactions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
