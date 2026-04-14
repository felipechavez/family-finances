namespace FinanceApp.Application.Features.Families.JoinFamily;
using FinanceApp.Application.Common;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Families;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Entities;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Supabase;

/// <summary>
/// Handles <see cref="JoinFamilyCommand"/>: validates the family exists, adds the user as a member,
/// and issues a new JWT that embeds the <c>family_id</c> claim.
/// </summary>
public class JoinFamilyHandler(Client supabase, IJwtService jwt, ILogger<JoinFamilyHandler> logger)
    : IRequestHandler<JoinFamilyCommand, FamilySetupResult>
{
    public async Task<FamilySetupResult> Handle(JoinFamilyCommand request, CancellationToken cancellationToken)
    {
        var familyResponse = await supabase.From<Family>()
            .Where(f => f.Id == request.FamilyId)
            .Get();

        _ = familyResponse.Model
            ?? throw new AppException(LocalizationKeys.Family_NotFound, 404);

        var existingResponse = await supabase.From<FamilyMember>()
            .Where(m => m.UserId == request.UserId && m.FamilyId == request.FamilyId)
            .Get();

        if (existingResponse.Model == null)
        {
            var member = FamilyMember.Create(request.FamilyId, request.UserId, FamilyRole.Member);
            await supabase.From<FamilyMember>().Insert(member);
        }

        var userResponse = await supabase.From<Users>()
            .Where(u => u.Id == request.UserId)
            .Get();
        var user = userResponse.Model!;

        var token = jwt.GenerateToken(user, request.FamilyId, FamilyRole.Member.ToString());

        logger.LogInformation("User {UserId} joined family {FamilyId}", request.UserId, request.FamilyId);

        return new FamilySetupResult(token, request.FamilyId);
    }
}
