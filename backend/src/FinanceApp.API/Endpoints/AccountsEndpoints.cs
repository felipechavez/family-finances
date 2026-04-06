namespace FinanceApp.API.Endpoints;
using System.Security.Claims;
using FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Application.Features.Accounts.GetAccounts;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Maps financial account endpoints.
/// </summary>
internal static class AccountsEndpoints
{
    /// <summary>Registers all <c>/accounts</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/accounts").WithTags("Accounts").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAccountsQuery(user.GetFamilyId()));
            return Results.Ok(result);
        })
        .WithName("GetAccounts")
        .Produces<IReadOnlyList<AccountDto>>();

        group.MapPost("/", async (
            [FromBody] CreateAccountRequest req,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateAccountCommand(
                user.GetFamilyId(),
                req.Name,
                Enum.Parse<AccountType>(req.Type, ignoreCase: true),
                req.InitialBalance));

            return Results.Created($"/accounts/{id}", new { id });
        })
        .WithName("CreateAccount")
        .Produces<object>(201)
        .ProducesProblem(400);

        return app;
    }
}

/// <summary>Request body for creating a new account.</summary>
/// <param name="Name">The display name of the account.</param>
/// <param name="Type">The account type string (Cash, Bank, Savings, CreditCard).</param>
/// <param name="InitialBalance">The starting balance. Defaults to zero.</param>
internal record CreateAccountRequest(string Name, string Type, decimal InitialBalance = 0);
