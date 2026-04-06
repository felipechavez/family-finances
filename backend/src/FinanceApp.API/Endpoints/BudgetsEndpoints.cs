namespace FinanceApp.API.Endpoints;
using System.Security.Claims;
using FinanceApp.Application.Common.Interfaces;
using FinanceApp.Application.Features.Budgets.GetBudgets;
using FinanceApp.Application.Features.Budgets.UpsertBudget;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Maps budget endpoints: list and upsert.
/// </summary>
internal static class BudgetsEndpoints
{
    /// <summary>Registers all <c>/budgets</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapBudgetsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/budgets").WithTags("Budgets").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBudgetsQuery(user.GetFamilyId()));
            return Results.Ok(result);
        })
        .WithName("GetBudgets")
        .Produces<IReadOnlyList<BudgetDto>>();

        group.MapPost("/", async (
            UpsertBudgetRequest req,
            ClaimsPrincipal user,
            IMediator mediator,
            IAppDbContext db) =>
        {
            var familyId = user.GetFamilyId();
            var categoryId = await CategoryHelper.FindByNameAsync(db, req.Category, familyId);
            var id = await mediator.Send(new UpsertBudgetCommand(familyId, categoryId, req.Limit));
            return Results.Ok(new { id });
        })
        .WithName("UpsertBudget")
        .Produces<object>()
        .ProducesProblem(400);

        return app;
    }
}

/// <summary>Request body for creating or updating a budget.</summary>
/// <param name="Category">The category name to budget for.</param>
/// <param name="Limit">The monthly spending limit. Must be non-negative.</param>
internal record UpsertBudgetRequest(string Category, decimal Limit);
