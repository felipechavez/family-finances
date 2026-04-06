namespace FinanceApp.API.Endpoints;
using System.Security.Claims;
using FinanceApp.Application.Features.Reports.GetMonthlySummary;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Maps financial report endpoints.
/// </summary>
internal static class ReportsEndpoints
{
    /// <summary>Registers all <c>/reports</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports").WithTags("Reports").RequireAuthorization();

        group.MapGet("/monthly", async (
            ClaimsPrincipal user,
            IMediator mediator,
            [FromQuery] int? year,
            [FromQuery] int? month) =>
        {
            var now = DateTime.UtcNow;
            var result = await mediator.Send(new GetMonthlySummaryQuery(
                user.GetFamilyId(),
                year ?? now.Year,
                month ?? now.Month));

            return Results.Ok(result);
        })
        .WithName("GetMonthlySummary")
        .Produces<MonthlySummaryDto>();

        return app;
    }
}
