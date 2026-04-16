namespace FinanceApp.API.Endpoints;
using Microsoft.AspNetCore.Mvc;
using FinanceApp.Application.Features.Auth.Confirm2Fa;
using FinanceApp.Application.Features.Auth.Disable2Fa;
using FinanceApp.Application.Features.Auth.GetUserProfile;
using FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Features.Auth.ResendVerification;
using FinanceApp.Application.Features.Auth.Setup2Fa;
using FinanceApp.Application.Features.Auth.Verify2Fa;
using FinanceApp.Application.Features.Auth.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

/// <summary>
/// Maps authentication endpoints: registration, login, email verification, and 2FA management.
/// </summary>
internal static class AuthEndpoints
{
    /// <summary>Registers all <c>/auth</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        // ── Registration & Login ────────────────────────────────────────────────

        group.MapPost("/register", async (RegisterCommand cmd, IMediator mediator) =>
        {
            var result = await mediator.Send(cmd);
            return Results.Created($"/users/{result.UserId}", result);
        })
        .WithName("Register")
        .Produces<RegisterResult>(201)
        .ProducesProblem(400)
        .ProducesProblem(409);

        group.MapPost("/login", async (LoginCommand cmd, IMediator mediator) =>
        {
            var result = await mediator.Send(cmd);
            return Results.Ok(result);
        })
        .WithName("Login")
        .Produces<LoginResult>()
        .ProducesProblem(401)
        .ProducesProblem(403);

        // ── Email Verification ─────────────────────────────────────────────────

        group.MapPost("/verify-email", async (VerifyEmailRequest req, IMediator mediator) =>
        {
            await mediator.Send(new VerifyEmailCommand(req.Token));
            return Results.Ok();
        })
        .WithName("VerifyEmail")
        .ProducesProblem(400)
        .ProducesProblem(409);

        group.MapPost("/resend-verification", async (ResendVerificationRequest req, IMediator mediator) =>
        {
            await mediator.Send(new ResendVerificationCommand(req.Email));
            return Results.Ok(); // Always 200 to prevent enumeration
        })
        .WithName("ResendVerification");

        // ── 2FA Management (requires authentication) ──────────────────────────

        var secured = app.MapGroup("/auth").WithTags("Auth").RequireAuthorization();

        secured.MapGet("/me", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.GetUserId();
            var result = await mediator.Send(new GetUserProfileQuery(userId));
            return Results.Ok(result);
        })
        .WithName("GetUserProfile")
        .Produces<UserProfileResult>();

        secured.MapPost("/setup-2fa", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.GetUserId();
            var result = await mediator.Send(new Setup2FaCommand(userId));
            return Results.Ok(result);
        })
        .WithName("Setup2Fa")
        .Produces<Setup2FaResult>()
        .ProducesProblem(409);

        secured.MapPost("/confirm-2fa", async (Confirm2FaRequest req, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.GetUserId();
            await mediator.Send(new Confirm2FaCommand(userId, req.Code));
            return Results.Ok();
        })
        .WithName("Confirm2Fa")
        .ProducesProblem(400)
        .ProducesProblem(409);

        secured.MapDelete("/disable-2fa", async ([FromBody] Disable2FaRequest req, ClaimsPrincipal user, IMediator mediator) =>
        {
            var userId = user.GetUserId();
            await mediator.Send(new Disable2FaCommand(userId, req.Code));
            return Results.Ok();
        })
        .WithName("Disable2Fa")
        .ProducesProblem(400)
        .ProducesProblem(409);

        // ── 2FA Verification (completes the 2FA login challenge) ───────────────

        group.MapPost("/verify-2fa", async (Verify2FaCommand cmd, IMediator mediator) =>
        {
            var result = await mediator.Send(cmd);
            return Results.Ok(result);
        })
        .WithName("Verify2Fa")
        .Produces<LoginResult>()
        .ProducesProblem(401);

        return app;
    }
}

/// <summary>Request body for email verification.</summary>
internal record VerifyEmailRequest(string Token);

/// <summary>Request body for resending a verification email.</summary>
internal record ResendVerificationRequest(string Email);

/// <summary>Request body for confirming 2FA setup.</summary>
internal record Confirm2FaRequest(string Code);

/// <summary>Request body for disabling 2FA.</summary>
internal record Disable2FaRequest(string Code);
