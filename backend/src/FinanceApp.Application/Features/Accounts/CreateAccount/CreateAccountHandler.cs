namespace FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Families.CreateFamily;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="CreateAccountCommand"/>: creates and persists a new financial account.
/// </summary>

public class CreateFamilyHandler(Client supabase, ILogger<CreateFamilyHandler> logger)
    : IRequestHandler<CreateFamilyCommand, Guid>
{
    public async Task<Guid> Handle(CreateFamilyCommand request, CancellationToken cancellationToken)
    {
        var family = Family.Create(request.Name, request.OwnerUserId);
        family.AddMember(request.OwnerUserId, FamilyRole.Owner);

        await supabase.From<Family>().Insert(family);

        logger.LogInformation("Family {FamilyId} created by user {UserId}", family.Id, request.OwnerUserId);

        return family.Id;
    }
}