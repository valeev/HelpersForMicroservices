namespace Infrastructure.Api;

/// <summary>
/// Self hosted services health check
/// </summary>
[ExcludeFromCodeCoverage]
public class HostedServiceHealthCheck : IHealthCheck
{
    /// <summary>
    /// Name
    /// </summary>
    public static string Name => "slow_dependency_check";

    /// <summary>
    /// List of background services and their status
    /// </summary>
    public Dictionary<string, bool> BackgroundServices { get; set; } = new();

    /// <summary>
    /// Check is process is ended
    /// </summary>
    /// <returns></returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (BackgroundServices.All(t => t.Value))
        {
            return Task.FromResult(HealthCheckResult.Healthy("The background services are still running"));
        }
        return Task.FromResult(HealthCheckResult.Unhealthy("One or all background services are not running"));
    }
}