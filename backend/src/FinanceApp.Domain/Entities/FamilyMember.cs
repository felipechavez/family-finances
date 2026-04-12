using Supabase.Postgrest.Attributes;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace FinanceApp.Domain.Entities;

[Table("family_members")]
public class FamilyMember : Entity
{
    [Column("family_id")]
    [JsonInclude]
    [JsonPropertyName("family_id")]
    public Guid FamilyId { get; set; }

    [Column("user_id")]
    [JsonInclude]
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    [Column("role")]
    [JsonInclude]
    [JsonPropertyName("role")]
    public FamilyRole Role { get; set; }

    public static FamilyMember Create(Guid familyId, Guid userId, FamilyRole role)
        => new() { FamilyId = familyId, UserId = userId, Role = role };
}
