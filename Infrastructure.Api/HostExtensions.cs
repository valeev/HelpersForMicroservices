namespace Infrastructure.Api;

/// <summary>
/// Host custom extensions
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Migrate DB context
    /// </summary>
    /// <returns></returns>
    public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();
            try
            {
                logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                var retries = 10;
                var polyRetry = Policy.Handle<SqlException>()
                    .WaitAndRetry(
                        retryCount: retries,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, timeSpan, retry, ctx) =>
                        {
                            logger.LogWarning(exception, $"[{nameof(TContext)}] Exception {exception.GetType().Name} with message {exception.Message} detected on attempt {retry} of {retries}");
                        });
                polyRetry.Execute(() => InvokeSeeder(seeder, context, services));

                logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
            }
        }
        return host;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
        where TContext : DbContext
    {
        context.Database.Migrate();
        seeder(context, services);
    }
}
