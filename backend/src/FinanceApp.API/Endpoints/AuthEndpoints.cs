namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Maps authentication endpoints: registration and login.
/// </summary>
internal static class AuthEndpoints
{
    /// <summary>Registers all <c>/auth</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterCommand cmd, IMediator mediator) =>
        {
            var result = await mediator.Send(cmd);
            return Results.Created($"/users/{result.UserId}", result);
        })
        .WithName("Register")
        .Produces<RegisterResult>(201)
        .ProducesProblem(400);

        group.MapPost("/login", async (LoginCommand cmd, IMediator mediator) =>
        {
            var result = await mediator.Send(cmd);
            return Results.Ok(result);
        })
        .WithName("Login")
        .Produces<LoginResult>()
        .ProducesProblem(401);

        return app;
    }
}
