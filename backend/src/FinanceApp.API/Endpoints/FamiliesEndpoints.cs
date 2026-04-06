namespace FinanceApp.API.Endpoints;
using System.Security.Claims;
using FinanceApp.Application.Features.Families.CreateFamily;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Maps family management endpoints.
/// </summary>
internal static class FamiliesEndpoints
{
    /// <summary>Registers all <c>/families</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapFamiliesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/families").WithTags("Families").RequireAuthorization();

        group.MapPost("/", async (CreateFamilyCommand cmd, IMediator mediator) =>
        {
            var id = await mediator.Send(cmd);
            return Results.Created($"/families/{id}", new { id });
        })
        .WithName("CreateFamily")
        .Produces<object>(201)
        .ProducesProblem(400);

        return app;
    }
}
