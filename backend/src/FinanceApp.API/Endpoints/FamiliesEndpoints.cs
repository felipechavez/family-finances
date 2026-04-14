namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Features.Families;
using FinanceApp.Application.Features.Families.CreateFamily;
using FinanceApp.Application.Features.Families.JoinFamily;
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

        return app;
    }
}

/// <summary>Request body for creating a new family.</summary>
internal record CreateFamilyRequest(string Name);

/// <summary>Request body for joining an existing family by its identifier.</summary>
internal record JoinFamilyRequest(Guid FamilyId);
