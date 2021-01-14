using System.Threading;
using System.Threading.Tasks;
using Marketing.Api.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Marketing.Api.AddHealthChecks
{
    public class StorageHealthCheck:IHealthCheck
    {
        private readonly IAdvertStorageService _storageService;

        public StorageHealthCheck(IAdvertStorageService storageService)
        {
            _storageService = storageService;
        }


        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var isStorageOk = await _storageService.CheckHealthAsync();
            return new HealthCheckResult(isStorageOk ? HealthStatus.Healthy : HealthStatus.Unhealthy);
        }
        
    }
}