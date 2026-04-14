using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinanceApp.Domain.Enums;

/// <summary>Classifies a transaction as money coming in or going out.</summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TransactionType
{
    /// <summary>Money received (salary, interest, etc.).</summary>
    [EnumMember(Value = "income")]
    Income,
    /// <summary>Money spent (purchases, bills, etc.).</summary>
    [EnumMember(Value = "expense")]
    Expense
}
