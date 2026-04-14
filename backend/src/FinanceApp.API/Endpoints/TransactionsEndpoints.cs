namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Common;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Application.Features.Transactions.DeleteTransaction;
using FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Domain.Common;
using FinanceApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Supabase;
using System.Net;
using System.Security.Claims;

/// <summary>
/// Maps transaction endpoints: list, create, and delete.
/// </summary>
internal static class TransactionsEndpoints
{
    /// <summary>Registers all <c>/transactions</c> routes on the provided route builder.</summary>
    internal static IEndpointRouteBuilder MapTransactionsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions").WithTags("Transactions").RequireAuthorization();

        group.MapGet("/", async (
            ClaimsPrincipal user,
            IMediator mediator,
            [FromQuery] int? month,
            [FromQuery] int? year) =>
        {
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);
            var result = await mediator.Send(new GetTransactionsQuery(familyId, month, year));
            return Results.Ok(result);
        })
        .WithName("GetTransactions")
        .Produces<IReadOnlyList<TransactionDto>>();

        group.MapPost("/", async (
            CreateTransactionRequest req,
            ClaimsPrincipal user,
            IMediator mediator,
            Client supabase) =>
        {
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);
            var categoryId = await CategoryHelper.FindByNameAsync(supabase, req.Category, familyId);
            var type = req.Type?.ToLowerInvariant() == "expense"
                ? TransactionType.Expense
                : TransactionType.Income;

            var cmd = new CreateTransactionCommand(
                familyId,
                req.AccountId,
                user.GetUserId(),
                categoryId,
                type,
                req.Amount,
                "CLP",
                req.Description,
                req.Date);

            var result = await mediator.Send(cmd);
            return Results.Created($"/transactions/{result.Id}", result);
        })
        .WithName("CreateTransaction")
        .Produces<TransactionDto>(201)
        .ProducesProblem(400);

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IMediator mediator) =>
        {
            var familyId = user.GetFamilyId()
                ?? throw new AppException(LocalizationKeys.Account_NoFamilyAssociated, (int)HttpStatusCode.NotFound);
            await mediator.Send(new DeleteTransactionCommand(id, familyId));
            return Results.NoContent();
        })
        .WithName("DeleteTransaction")
        .Produces(204)
        .ProducesProblem(404);

        return app;
    }
}

/// <summary>Request body for creating a new transaction.</summary>
/// <param name="AccountId">The account to record the transaction against.</param>
/// <param name="Category">The category name (must exist as global or family category).</param>
/// <param name="Type">Transaction direction: <c>"income"</c> or <c>"expense"</c>.</param>
/// <param name="Amount">The transaction amount. Must be positive.</param>
/// <param name="Description">A short description of the transaction.</param>
/// <param name="Date">The date the transaction occurred.</param>
internal record CreateTransactionRequest(
    Guid AccountId,
    string Category,
    string Type,
    decimal Amount,
    string Description,
    DateOnly Date);
