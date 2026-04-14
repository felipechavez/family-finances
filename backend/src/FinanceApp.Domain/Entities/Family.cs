using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;

namespace FinanceApp.Domain.Entities;
/// <summary>
/// Represents a family group that shares financial accounts, budgets, and transactions.
/// </summary>
[Table("families")]
public class Family : Entity
{
    [Column("name")]
    public string Name { get; set; } = default!;

    [Column("owner_user_id")]
    [JsonProperty("owner_user_id")]
    public Guid OwnerUserId { get; set; }

    private readonly List<FamilyMember> _members = [];
    [JsonIgnore]
    public IReadOnlyCollection<FamilyMember> Members => _members.AsReadOnly();

    public static Family Create(string name, Guid ownerUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Family { Name = name, OwnerUserId = ownerUserId };
    }
    public void AddMember(Guid userId, FamilyRole role = FamilyRole.Member)
    {
        if (_members.Any(m => m.UserId == userId)) return;
        _members.Add(FamilyMember.Create(Id, userId, role));
    }

}
