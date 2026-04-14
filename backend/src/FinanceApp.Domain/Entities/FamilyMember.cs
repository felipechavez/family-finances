using FinanceApp.Domain.Enums;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FinanceApp.Domain.Entities;

[Table("family_members")]
public class FamilyMember : BaseModel
{
    [Column("family_id")]
    [JsonProperty("family_id")]
    public Guid FamilyId { get; set; }

    [Column("user_id")]
    [JsonProperty("user_id")]
    public Guid UserId { get; set; }

    [Column("role")]
    [JsonProperty("role")]
    public FamilyRole Role { get; set; }

    [Column("joined_at")]
    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public static FamilyMember Create(Guid familyId, Guid userId, FamilyRole role)
        => new() { FamilyId = familyId, UserId = userId, Role = role };
}
