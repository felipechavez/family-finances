using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FinanceApp.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides a unique identifier and audit timestamp.
/// </summary>
public abstract class Entity : BaseModel
{
    /// <summary>Gets the unique identifier for this entity.</summary>
    [PrimaryKey("id", false)]
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>Gets the UTC timestamp when this entity was created.</summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
}
