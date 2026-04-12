namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Common;
using FinanceApp.Application.Features.Reports.GetMonthlySummary;
using FinanceApp.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net;
using System.Security.Claims;

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
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);
            var result = await mediator.Send(new GetMonthlySummaryQuery(
                familyId,
                year ?? now.Year,
                month ?? now.Month));
            return Results.Ok(result);
        })
        .WithName("GetMonthlySummary")
        .Produces<MonthlySummaryDto>();

        return app;
    }
}
