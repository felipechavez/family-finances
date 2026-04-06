namespace FinanceApp.Application.Features.Families.CreateFamily;
using MediatR;

/// <summary>Command to create a new family group and assign the creator as its owner.</summary>
/// <param name="Name">The display name of the family group.</param>
/// <param name="OwnerUserId">The identifier of the user who will own the family.</param>
public record CreateFamilyCommand(string Name, Guid OwnerUserId) : IRequest<Guid>;
