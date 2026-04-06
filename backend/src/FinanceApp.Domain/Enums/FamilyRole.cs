namespace FinanceApp.Domain.Enums;

/// <summary>Defines the roles a user can hold within a <see cref="FinanceApp.Domain.Entities.Family"/>.</summary>
public enum FamilyRole
{
    /// <summary>The user who created the family and has full administrative rights.</summary>
    Owner,
    /// <summary>A user with elevated permissions, able to manage members and settings.</summary>
    Admin,
    /// <summary>A standard family member with read/write access to shared finances.</summary>
    Member
}
