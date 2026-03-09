namespace FinanceApp.Domain.Entities;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;

public class FamilyMember : Entity
{
    public Guid FamilyId { get; private set; }
    public Guid UserId { get; private set; }
    public FamilyRole Role { get; private set; }

    private FamilyMember() { }

    public static FamilyMember Create(Guid familyId, Guid userId, FamilyRole role)
        => new() { FamilyId = familyId, UserId = userId, Role = role };
}
