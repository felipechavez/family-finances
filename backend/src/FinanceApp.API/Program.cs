using FinanceApp.API.Endpoints;
using FinanceApp.API.Initializations;
using FinanceApp.API.Resources;
using FinanceApp.Application;
using FinanceApp.Domain.Common;
using FinanceApp.Infrastructure;
using Microsoft.Extensions.Localization;

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

builder.Services.AddLocalization();

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

SupabaseConfiguration.Initialize(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Eliminado código de migraciones y acceso a AppDbContext
}

app.UseRequestLocalization(options =>
{
    var supportedCultures = new[] { "en", "es" };
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
    
    // Reemplazar los proveedores con solo el AcceptLanguageHeaderRequestCultureProvider
    options.RequestCultureProviders.Clear();
    options.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        // Obtener la cultura actual del contexto HTTP
        var cultureFeature = context.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
        var culture = cultureFeature?.RequestCulture.Culture ?? System.Globalization.CultureInfo.InvariantCulture;
        
        // Establecer la cultura ANTES de obtener el localizer
        System.Globalization.CultureInfo.CurrentCulture = culture;
        System.Globalization.CultureInfo.CurrentUICulture = culture;
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

        var localizer = context.RequestServices
            .GetRequiredService<IStringLocalizer<SharedResource>>();

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex switch
        {
            AppException appEx => appEx.StatusCode,
            FluentValidation.ValidationException => 400,
            UnauthorizedAccessException => 401,
            KeyNotFoundException => 404,
            _ => 500
        };
        
        var message = ex switch
        {
            AppException appEx => localizer[appEx.ResourceKey, appEx.Args].Value,
            FluentValidation.ValidationException ve =>
                LocalizedFluentValidator.GetLocalizedValidationErrors(ve, localizer),
            _ => ex?.Message ?? localizer["ErrorUnexpected"].Value
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
app.MapNotificationsEndpoints();

app.Run();

public partial class Program { } // for integration tests
