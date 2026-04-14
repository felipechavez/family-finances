using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinanceApp.Domain.Enums;

/// <summary>Defines the supported financial account types.</summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum AccountType
{
    /// <summary>Physical cash held outside any financial institution.</summary>
    [EnumMember(Value = "cash")]
    Cash,
    /// <summary>A standard checking or current bank account.</summary>
    [EnumMember(Value = "bank")]
    Bank,
    /// <summary>A savings or deposit account.</summary>
    [EnumMember(Value = "savings")]
    Savings,
    /// <summary>A credit card account (balance represents outstanding debt).</summary>
    [EnumMember(Value = "credit_card")]
    CreditCard
}
