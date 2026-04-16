namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Features.Families;
using FinanceApp.Application.Features.Families.CreateFamily;
using FinanceApp.Application.Features.Families.GetFamilyInfo;
using FinanceApp.Application.Features.Families.JoinByCode;
using FinanceApp.Application.Features.Families.JoinFamily;
using FinanceApp.Application.Features.Families.RegenerateInviteCode;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

/// <summary>
/// Maps family management endpoints.
/// </summary>
internal static class FamiliesEndpoints
{
    /// <summary>Registers all <c>/families</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapFamiliesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/families").WithTags("Families").RequireAuthorization();

        // ── Create ────────────────────────────────────────────────────────────
        group.MapPost("/", async (
            CreateFamilyRequest req,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userId = user.GetUserId();
            var result = await mediator.Send(new CreateFamilyCommand(req.Name, userId));
            return Results.Created($"/families/{result.FamilyId}", result);
        })
        .WithName("CreateFamily")
        .Produces<FamilySetupResult>(201)
        .ProducesProblem(400);

        // ── Join by UUID (legacy) ─────────────────────────────────────────────
        group.MapPost("/join", async (
            JoinFamilyRequest req,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userId = user.GetUserId();
            var result = await mediator.Send(new JoinFamilyCommand(req.FamilyId, userId));
            return Results.Ok(result);
        })
        .WithName("JoinFamily")
        .Produces<FamilySetupResult>()
        .ProducesProblem(404);

        // ── Join by invite code ───────────────────────────────────────────────
        group.MapPost("/join-by-code", async (
            JoinByCodeRequest req,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userId = user.GetUserId();
            var result = await mediator.Send(new JoinByCodeCommand(req.Code, userId));
            return Results.Ok(result);
        })
        .WithName("JoinFamilyByCode")
        .Produces<FamilySetupResult>()
        .ProducesProblem(404);

        // ── Get current family info (members + invite code) ───────────────────
        group.MapGet("/me", async (
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userId   = user.GetUserId();
            var familyId = user.GetFamilyId()
                ?? throw new UnauthorizedAccessException("No family associated with this token.");
            var result = await mediator.Send(new GetFamilyInfoQuery(familyId, userId));
            return Results.Ok(result);
        })
        .WithName("GetFamilyInfo")
        .Produces<FamilyInfoResult>()
        .ProducesProblem(404);

        // ── Regenerate invite code (owner only) ───────────────────────────────
        group.MapPost("/me/regenerate-code", async (
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var userId   = user.GetUserId();
            var familyId = user.GetFamilyId()
                ?? throw new UnauthorizedAccessException("No family associated with this token.");
            var newCode = await mediator.Send(new RegenerateInviteCodeCommand(familyId, userId));
            return Results.Ok(new { inviteCode = newCode });
        })
        .WithName("RegenerateInviteCode")
        .Produces<object>()
        .ProducesProblem(403)
        .ProducesProblem(404);

        return app;
    }
}

/// <summary>Request body for creating a new family.</summary>
internal record CreateFamilyRequest(string Name);

/// <summary>Request body for joining an existing family by its identifier.</summary>
internal record JoinFamilyRequest(Guid FamilyId);

/// <summary>Request body for joining a family using a short invite code.</summary>
internal record JoinByCodeRequest(string Code);
