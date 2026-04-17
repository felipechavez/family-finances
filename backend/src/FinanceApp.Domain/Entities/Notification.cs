using FinanceApp.Domain.Common;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;

[Table("notifications")]
public class Notification : Entity
{
    [Column("user_id")]
    [JsonProperty("user_id")]
    public Guid UserId { get; set; }

    [Column("family_id")]
    [JsonProperty("family_id")]
    public Guid FamilyId { get; set; }

    /// <summary>"budget_exceeded" or "daily_summary"</summary>
    [Column("type")]
    [JsonProperty("type")]
    public string Type { get; set; } = default!;

    [Column("title")]
    [JsonProperty("title")]
    public string Title { get; set; } = default!;

    [Column("body")]
    [JsonProperty("body")]
    public string? Body { get; set; }

    [Column("is_read")]
    [JsonProperty("is_read")]
    public bool IsRead { get; set; } = false;

    public Notification() { }

    public static Notification Create(Guid userId, Guid familyId, string type, string title, string? body = null)
        => new() { UserId = userId, FamilyId = familyId, Type = type, Title = title, Body = body };
}
