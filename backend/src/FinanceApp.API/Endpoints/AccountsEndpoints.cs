namespace FinanceApp.API.Endpoints;

using System.Net;
using System.Security.Claims;
using FinanceApp.API.Resources;
using FinanceApp.Application.Common;
using FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Application.Features.Accounts.GetAccounts;
using FinanceApp.Application.Features.Accounts.UpdateAccount;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;

/// <summary>
/// Maps financial account endpoints.
/// </summary>
internal static class AccountsEndpoints
{
    /// <summary>Registers all <c>/accounts</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/accounts").WithTags("Accounts").RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal user,
            IStringLocalizer<SharedResource> localizer,
            IMediator mediator) =>
        {
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);

            var result = await mediator.Send(new GetAccountsQuery(familyId));
            return Results.Ok(result);
        })
        .WithName("GetAccounts")
        .Produces<IReadOnlyList<AccountDto>>();

        group.MapPost("/", async (
            [FromBody] CreateAccountRequest req,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);
            var id = await mediator.Send(new CreateAccountCommand(
                familyId,
                req.Name,
                Enum.Parse<AccountType>(req.Type, ignoreCase: true),
                req.InitialBalance));

            return Results.Created($"/accounts/{id}", new { id });
        })
        .WithName("CreateAccount")
        .Produces<object>(201)
        .ProducesProblem(400);

        group.MapPatch("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateAccountRequest req,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);

            await mediator.Send(new UpdateAccountCommand(id, familyId, req.Name, req.Balance));
            return Results.NoContent();
        })
        .WithName("UpdateAccount")
        .Produces(204)
        .ProducesProblem(400)
        .ProducesProblem(404)
        .ProducesProblem(409);

        return app;
    }
}

/// <summary>Request body for updating an existing account.</summary>
/// <param name="Name">New display name.</param>
/// <param name="Balance">New current balance (may be negative for credit accounts).</param>
internal record UpdateAccountRequest(string Name, decimal Balance);

/// <summary>Request body for creating a new account.</summary>
/// <param name="Name">The display name of the account.</param>
/// <param name="Type">The account type string (Cash, Bank, Savings, CreditCard).</param>
/// <param name="InitialBalance">The starting balance. Defaults to zero.</param>
internal record CreateAccountRequest(string Name, string Type, decimal InitialBalance = 0);
