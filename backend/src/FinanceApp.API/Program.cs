using FinanceApp.API.Endpoints;
using FinanceApp.Application;
using FinanceApp.Infrastructure;
// using FinanceApp.Infrastructure.Persistence; // Eliminado EF Core
// using Microsoft.EntityFrameworkCore; // Eliminado EF Core
using Supabase;

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

// Eliminado HealthChecks de EF Core
// builder.Services.AddHealthChecks()
//     .AddDbContextCheck<AppDbContext>("database");

var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true,
    // SessionHandler = new SupabaseSessionHandler() <-- This must be implemented by el developer
};

builder.Services.AddSingleton(provider => new Client(url!, key, options));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Eliminado código de migraciones y acceso a AppDbContext
}

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

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapAuthEndpoints();
app.MapFamiliesEndpoints();
app.MapAccountsEndpoints();
app.MapTransactionsEndpoints();
app.MapBudgetsEndpoints();
app.MapReportsEndpoints();

app.Run();

public partial class Program { } // for integration tests
