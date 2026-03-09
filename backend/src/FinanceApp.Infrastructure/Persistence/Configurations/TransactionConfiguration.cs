namespace FinanceApp.Infrastructure.Persistence.Configurations;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.FamilyId).IsRequired();
        builder.Property(t => t.AccountId).IsRequired();
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.CategoryId).IsRequired();
        builder.Property(t => t.Type).HasConversion<string>().IsRequired();
        builder.Property(t => t.Amount).HasColumnType("numeric(15,2)").IsRequired();
        builder.Property(t => t.Currency).HasMaxLength(3).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(200).IsRequired();
        builder.Property(t => t.TransactionDate).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasIndex(t => t.FamilyId);
        builder.HasIndex(t => t.TransactionDate);
        builder.HasIndex(t => t.CategoryId);
    }
}
