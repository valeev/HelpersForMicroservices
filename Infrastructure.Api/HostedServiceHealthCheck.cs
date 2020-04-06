using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Api
{
    /// <summary>
    /// Self hosted services health check
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HostedServiceHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name => "slow_dependency_check";

        /// <summary>
        /// List of background services and their status
        /// </summary>
        public Dictionary<string, bool> BackgroundServices { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Check is process is ended
        /// </summary>
        /// <returns></returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (BackgroundServices.All(t => t.Value))
            {
                return Task.FromResult(HealthCheckResult.Healthy("The background services are still running"));
            }
            return Task.FromResult(HealthCheckResult.Unhealthy("One or all background services are not running"));
        }
    }
}
