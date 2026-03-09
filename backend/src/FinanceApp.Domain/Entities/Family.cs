namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

public class Family : Entity
{
    public string Name { get; private set; } = default!;
    public Guid OwnerUserId { get; private set; }

    private readonly List<FamilyMember> _members = [];
    public IReadOnlyCollection<FamilyMember> Members => _members.AsReadOnly();

    private Family() { }

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
