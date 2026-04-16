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

    [Column("invite_code")]
    [JsonProperty("invite_code")]
    public string InviteCode { get; set; } = default!;

    private readonly List<FamilyMember> _members = [];
    [JsonIgnore]
    public IReadOnlyCollection<FamilyMember> Members => _members.AsReadOnly();

    /// <summary>Characters used for invite code generation — no ambiguous chars (0/O, 1/I/L).</summary>
    private const string CodeChars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public static Family Create(string name, Guid ownerUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Family { Name = name, OwnerUserId = ownerUserId, InviteCode = GenerateCode() };
    }

    /// <summary>Generates a fresh 8-character invite code.</summary>
    public static string GenerateCode()
    {
        Span<char> code = stackalloc char[8];
        for (var i = 0; i < 8; i++)
            code[i] = CodeChars[Random.Shared.Next(CodeChars.Length)];
        return new string(code);
    }
    public void AddMember(Guid userId, FamilyRole role = FamilyRole.Member)
    {
        if (_members.Any(m => m.UserId == userId)) return;
        _members.Add(FamilyMember.Create(Id, userId, role));
    }

}
