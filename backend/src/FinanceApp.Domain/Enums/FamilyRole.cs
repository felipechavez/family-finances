using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinanceApp.Domain.Enums;

/// <summary>Defines the roles a user can hold within a <see cref="FinanceApp.Domain.Entities.Family"/>.</summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum FamilyRole
{
    /// <summary>The user who created the family and has full administrative rights.</summary>
    [EnumMember(Value = "owner")]
    Owner,
    /// <summary>A user with elevated permissions, able to manage members and settings.</summary>
    [EnumMember(Value = "admin")]
    Admin,
    /// <summary>A standard family member with read/write access to shared finances.</summary>
    [EnumMember(Value = "member")]
    Member
}
