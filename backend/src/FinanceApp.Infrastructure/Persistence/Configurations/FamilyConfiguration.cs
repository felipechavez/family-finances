namespace FinanceApp.Infrastructure.Persistence.Configurations;
using FinanceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FamilyConfiguration : IEntityTypeConfiguration<Family>
{
    public void Configure(EntityTypeBuilder<Family> builder)
    {
        builder.ToTable("families");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).HasMaxLength(120).IsRequired();
        builder.Property(f => f.OwnerUserId).IsRequired();
        builder.HasMany(f => f.Members).WithOne().HasForeignKey(m => m.FamilyId).OnDelete(DeleteBehavior.Cascade);
    }
}
