namespace FinanceApp.Infrastructure.Persistence.Configurations;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.FamilyId).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(120).IsRequired();
        builder.Property(a => a.Type).HasConversion<string>().IsRequired();
        builder.Property(a => a.Balance).HasColumnType("numeric(15,2)");
        builder.HasIndex(a => a.FamilyId);
    }
}
