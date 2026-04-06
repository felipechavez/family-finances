namespace FinanceApp.Domain.Enums;

/// <summary>Defines the supported financial account types.</summary>
public enum AccountType
{
    /// <summary>Physical cash held outside any financial institution.</summary>
    Cash,
    /// <summary>A standard checking or current bank account.</summary>
    Bank,
    /// <summary>A savings or deposit account.</summary>
    Savings,
    /// <summary>A credit card account (balance represents outstanding debt).</summary>
    CreditCard
}
