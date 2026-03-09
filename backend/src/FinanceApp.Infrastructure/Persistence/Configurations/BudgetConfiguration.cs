namespace FinanceApp.Infrastructure.Persistence.Configurations;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("budgets");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.FamilyId).IsRequired();
        builder.Property(b => b.CategoryId).IsRequired();
        builder.Property(b => b.MonthlyLimit).HasColumnType("numeric(15,2)").IsRequired();
        builder.HasIndex(b => new { b.FamilyId, b.CategoryId }).IsUnique();
    }
}
