namespace FinanceApp.Application.Features.Families.CreateFamily;
using MediatR;

public record CreateFamilyCommand(string Name, Guid OwnerUserId) : IRequest<Guid>;
