namespace FinanceApp.Infrastructure.Persistence.Configurations;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(80).IsRequired();
        builder.Property(c => c.Type).HasConversion<string>().IsRequired();
        builder.Property(c => c.FamilyId);
        builder.HasIndex(c => new { c.FamilyId, c.Name }).IsUnique();
    }
}
