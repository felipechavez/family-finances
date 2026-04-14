using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FinanceApp.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides a unique identifier and audit timestamp.
/// </summary>
public abstract class Entity : BaseModel
{
    /// <summary>Gets the unique identifier for this entity.</summary>
    [PrimaryKey("id", true)]
    [JsonProperty("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets the UTC timestamp when this entity was created.</summary>
    [Column("created_at")]
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
