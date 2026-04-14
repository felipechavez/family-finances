using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinanceApp.Domain.Enums;

/// <summary>Defines how frequently a <see cref="FinanceApp.Domain.Entities.RecurringTransaction"/> repeats.</summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum RecurrenceType
{
    /// <summary>Repeats every day.</summary>
    [EnumMember(Value = "daily")]
    Daily,
    /// <summary>Repeats every week.</summary>
    [EnumMember(Value = "weekly")]
    Weekly,
    /// <summary>Repeats every calendar month.</summary>
    [EnumMember(Value = "monthly")]
    Monthly,
    /// <summary>Repeats every year.</summary>
    [EnumMember(Value = "yearly")]
    Yearly
}
