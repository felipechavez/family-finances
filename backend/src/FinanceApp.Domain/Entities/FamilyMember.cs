using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

[Table("family_members")]
public class FamilyMember : Entity
{
    [Column("family_id")]
    public Guid FamilyId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("role")]
    public FamilyRole Role { get; set; }

    public static FamilyMember Create(Guid familyId, Guid userId, FamilyRole role)
        => new() { FamilyId = familyId, UserId = userId, Role = role };
}
