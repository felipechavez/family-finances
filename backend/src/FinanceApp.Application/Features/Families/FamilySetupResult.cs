namespace FinanceApp.Application.Features.Families;

/// <summary>Result returned after a user successfully creates or joins a family.</summary>
/// <param name="Token">A new JWT token that embeds the resolved <paramref name="FamilyId"/> claim.</param>
/// <param name="FamilyId">The identifier of the family the user now belongs to.</param>
public record FamilySetupResult(string Token, Guid FamilyId);
