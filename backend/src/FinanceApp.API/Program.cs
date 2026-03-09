using System.Security.Claims;
using FinanceApp.Application;
using FinanceApp.Application.Features.Auth.Login;
using FinanceApp.Application.Features.Auth.Register;
using FinanceApp.Application.Features.Accounts.CreateAccount;
using FinanceApp.Application.Features.Accounts.GetAccounts;
using FinanceApp.Application.Features.Budgets.GetBudgets;
using FinanceApp.Application.Features.Budgets.UpsertBudget;
using FinanceApp.Application.Features.Families.CreateFamily;
using FinanceApp.Application.Features.Reports.GetMonthlySummary;
using FinanceApp.Application.Features.Transactions.CreateTransaction;
using FinanceApp.Application.Features.Transactions.DeleteTransaction;
using FinanceApp.Application.Features.Transactions.GetTransactions;
using FinanceApp.Domain.Enums;
using FinanceApp.Infrastructure;
using FinanceApp.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FinanceApp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using Bearer scheme",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            []
        }
    });
});

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Auto-migrate in dev
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// ── Health ──
app.MapHealthChecks("/health");

// ── Auth ──
var auth = app.MapGroup("/auth").WithTags("Auth");

auth.MapPost("/register", async (RegisterCommand cmd, IMediator mediator) =>
{
    var result = await mediator.Send(cmd);
    return Results.Created($"/users/{result.UserId}", result);
})
.WithName("Register")
.Produces<RegisterResult>(201)
.ProducesProblem(400);

auth.MapPost("/login", async (LoginCommand cmd, IMediator mediator) =>
{
    var result = await mediator.Send(cmd);
    return Results.Ok(result);
})
.WithName("Login")
.Produces<LoginResult>()
.ProducesProblem(401);

// ── Families ──
var families = app.MapGroup("/families").WithTags("Families").RequireAuthorization();

families.MapPost("/", async (CreateFamilyCommand cmd, IMediator mediator) =>
{
    var id = await mediator.Send(cmd);
    return Results.Created($"/families/{id}", new { id });
}).WithName("CreateFamily");

// ── Transactions ──
var transactions = app.MapGroup("/transactions").WithTags("Transactions").RequireAuthorization();

transactions.MapGet("/", async (
    ClaimsPrincipal user,
    IMediator mediator,
    [FromQuery] int? month,
    [FromQuery] int? year) =>
{
    var familyId = GetFamilyId(user);
    var result = await mediator.Send(new GetTransactionsQuery(familyId, month, year));
    return Results.Ok(result);
}).WithName("GetTransactions");

transactions.MapPost("/", async (
    CreateTransactionCommand cmd,
    ClaimsPrincipal user,
    IMediator mediator) =>
{
    var familyId = GetFamilyId(user);
    var userId = GetUserId(user);
    var result = await mediator.Send(cmd with { FamilyId = familyId, UserId = userId });
    return Results.Created($"/transactions/{result.Id}", result);
}).WithName("CreateTransaction");

transactions.MapDelete("/{id:guid}", async (
    Guid id,
    ClaimsPrincipal user,
    IMediator mediator) =>
{
    await mediator.Send(new DeleteTransactionCommand(id, GetFamilyId(user)));
    return Results.NoContent();
}).WithName("DeleteTransaction");

// ── Accounts ──
var accounts = app.MapGroup("/accounts").WithTags("Accounts").RequireAuthorization();

accounts.MapGet("/", async (ClaimsPrincipal user, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAccountsQuery(GetFamilyId(user)));
    return Results.Ok(result);
}).WithName("GetAccounts");

accounts.MapPost("/", async (
    [FromBody] CreateAccountRequest req,
    ClaimsPrincipal user,
    IMediator mediator) =>
{
    var id = await mediator.Send(new CreateAccountCommand(GetFamilyId(user), req.Name,
        Enum.Parse<AccountType>(req.Type, true), req.InitialBalance));
    return Results.Created($"/accounts/{id}", new { id });
}).WithName("CreateAccount");

// ── Budgets ──
var budgets = app.MapGroup("/budgets").WithTags("Budgets").RequireAuthorization();

budgets.MapGet("/", async (ClaimsPrincipal user, IMediator mediator) =>
{
    var result = await mediator.Send(new GetBudgetsQuery(GetFamilyId(user)));
    return Results.Ok(result);
}).WithName("GetBudgets");

budgets.MapPost("/", async (UpsertBudgetCommand cmd, ClaimsPrincipal user, IMediator mediator) =>
{
    var id = await mediator.Send(cmd with { FamilyId = GetFamilyId(user) });
    return Results.Ok(new { id });
}).WithName("UpsertBudget");

// ── Reports ──
var reports = app.MapGroup("/reports").WithTags("Reports").RequireAuthorization();

reports.MapGet("/monthly", async (
    ClaimsPrincipal user,
    IMediator mediator,
    [FromQuery] int? year,
    [FromQuery] int? month) =>
{
    var now = DateTime.UtcNow;
    var result = await mediator.Send(new GetMonthlySummaryQuery(
        GetFamilyId(user), year ?? now.Year, month ?? now.Month));
    return Results.Ok(result);
}).WithName("GetMonthlySummary");

// ── Global exception handler ──
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex switch
        {
            FluentValidation.ValidationException => 400,
            UnauthorizedAccessException => 401,
            KeyNotFoundException => 404,
            _ => 500
        };

        var message = ex switch
        {
            FluentValidation.ValidationException ve =>
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage)),
            _ => ex?.Message ?? "An unexpected error occurred"
        };

        await context.Response.WriteAsJsonAsync(new { error = message });
    });
});

app.Run();

// ── Helpers ──
static Guid GetFamilyId(ClaimsPrincipal user)
{
    var claim = user.FindFirstValue("family_id")
        ?? throw new UnauthorizedAccessException("No family associated with this user.");
    return Guid.Parse(claim);
}

static Guid GetUserId(ClaimsPrincipal user)
{
    var claim = user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedAccessException("User identity not found.");
    return Guid.Parse(claim);
}

record CreateAccountRequest(string Name, string Type, decimal InitialBalance = 0);

public partial class Program { } // for integration tests
