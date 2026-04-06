namespace FinanceApp.Domain.Enums;

/// <summary>Classifies a transaction as money coming in or going out.</summary>
public enum TransactionType
{
    /// <summary>Money received (salary, interest, etc.).</summary>
    Income,
    /// <summary>Money spent (purchases, bills, etc.).</summary>
    Expense
}
