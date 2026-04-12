using Microsoft.Extensions.Diagnostics.HealthChecks;
using Supabase;

namespace FinanceApp.API.Initializations
{
    public class SupabaseHealthCheck : IHealthCheck
    {
        private readonly Client _client;

        public SupabaseHealthCheck(Client client)
        {
            _client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Verificar que el cliente de Supabase esté inicializado
                if (_client == null)
                    return HealthCheckResult.Unhealthy("Supabase client is not initialized");

                // Intentar una operación simple (por ejemplo, obtener el estado de autenticación)
                // Esto asume que tienes acceso a una tabla o endpoint básico                
                await _client.InitializeAsync();

                return HealthCheckResult.Healthy("Supabase connection is healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Supabase health check failed: {ex.Message}", ex);
            }
        }
    }
}
