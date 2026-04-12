using Supabase;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FinanceApp.API.Initializations
{
    public static class SupabaseConfiguration
    {
        public static void Initialize(IServiceCollection services)
        {
            var url = Environment.GetEnvironmentVariable("SUPABASE_URL")
                ?? throw new InvalidOperationException("Environment variable SUPABASE_URL is required.");
            var key = Environment.GetEnvironmentVariable("SUPABASE_KEY")
                ?? throw new InvalidOperationException("Environment variable SUPABASE_KEY is required.");
            var schema = Environment.GetEnvironmentVariable("SUPABASE_SCHEMA") ?? "public";

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                Schema = schema
                // SessionHandler = new SupabaseSessionHandler() <-- This must be implemented by the developer
            };

            services.AddSingleton(provider => new Client(url!, key, options));
            
            // Registrar health check de Supabase
            services.AddHealthChecks()
                .AddCheck<SupabaseHealthCheck>("supabase", HealthStatus.Degraded);
        }
    }
}
